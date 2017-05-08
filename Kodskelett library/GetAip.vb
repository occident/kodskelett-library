Imports System.Security.Cryptography.X509Certificates
Imports System.ServiceModel
Imports System.Xml

Module GetAip
	''' <summary>
	''' This method returns the Archive.xml and the files that it has attached.
	''' </summary>
	''' <param name="id">The id of the Object that will be fetched from the system</param>
	''' <param name="oSettings">The settings object</param>
	''' <param name="oLogger">The logger where every log will be saved</param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function ssaGetAipObject(ByVal id As Integer, ByVal oSettings As Settings, ByVal oLogger As Logger) As SSA.GetAipResponse
		'--- Gets the client generated with the certificate
		Dim client As SSA.VirtualInterfaceClient = oSettings.getVirtualInterfaceClient()
		'--- Where the response will be saved
		Dim oGetAipResponse As New SSA.GetAipResponse

		Dim oGetAip As New SSA.GetAip
		oGetAip.Id = "iipax://objectbase.document/docpartition#" & id
		oGetAip.callerId = "?"

		'--- This is only if the advanced log is activated
		If oSettings.advancedLog Then
			oLogger.Log("[GetAip] Calling the server with: " & oGetAip.Id)
		End If

		Using scope As New OperationContextScope(client.InnerChannel)

			'--- Appending the headers to the request
			oSettings.AddSOAPMessageHeaders()
			'--- Prints the endpoint settings in the logger if it has advanced logging
			oSettings.printEndpointSettings(oLogger)

			Try
				'--- Object that stores the ArchiveS ip Object response from the server
				oGetAipResponse = client.GetAip(oGetAip)

				'--- Logging sucessfull message
				oLogger.Log("GetAIP(" & id & ") - Status: Success.")
			Catch ex As EndpointNotFoundException
				oLogger.Log("[Error] GetAip(" & id & ") - The endpoint is not valid.")
				If oSettings.advancedLog Then
					oLogger.Log("[Exception] - " & ex.Message.ToString)
				End If
			Catch fault As FaultException
				oLogger.Log("[Error] GetAip(" & id & ") - The ID might not be correct or does not exist on the system.")
				If oSettings.advancedLog Then
					oLogger.Log("[FaultException] - " & fault.Message.ToString)
				End If
			Catch sysEx As System.OutOfMemoryException
				oLogger.Log("[Error] GetAip(" & id & ") - The object you are trying to send is too large.")
				If oSettings.advancedLog Then
					oLogger.Log("[SystemException] - " & sysEx.Message.ToString)
				End If
			Catch timeEx As System.TimeoutException
				oLogger.Log("[Error] GetAip(" & id & ") - The time allocated for this process has expired.")
				If oSettings.advancedLog Then
					oLogger.Log("[TimeoutException] - " & timeEx.Message.ToString)
				End If
			End Try

		End Using

		client.Close()

		Return oGetAipResponse
	End Function


	''' <summary>
	''' Gets the Sip object and renders the results in the custom path assigned.
	''' </summary>
	''' <param name="id">The id of the Object that will be fetched from the system</param>
	''' <param name="oSettings">The settings object</param>
	''' <param name="oLogger">The logger where every log will be saved</param>
	''' <param name="dirPath">The path where the results will be saved</param>
	''' <remarks></remarks>
	Public Sub ssaGetAip(ByVal id As Integer, ByVal oSettings As Settings, ByVal oLogger As Logger, ByVal dirPath As String)
		'--- Call to the server
		Dim oGetAipResponse As SSA.GetAipResponse = ssaGetAipObject(id, oSettings, oLogger)

		'--- Creates the output directory if it does not exist
		If Not IO.File.Exists(dirPath) Then
			IO.Directory.CreateDirectory(dirPath)
		End If

		'--- Some advanced Logging
		If oSettings.advancedLog Then
			oLogger.Log("Generating Archive.xml from object...")
		End If
		'--- Gets the Archive.xml and saves to disk
		ssaGetAipAsXml(oGetAipResponse, oSettings, oLogger).Save(IO.Path.Combine(dirPath, "Archive.xml"))
		'--- Some advanced Logging
		If oSettings.advancedLog Then
			oLogger.Log("File saved to disk on path: " & dirPath & "Archive.xml")
		End If


		'--- Helps to know what kind of variable is going to be
		Dim objectResponse = oGetAipResponse.ArchiveObject.Items(0)

		'--- It has a File structure (No document)
		If TypeOf objectResponse Is SSA.File Then

			For Each fileInObj As SSA.File In oGetAipResponse.ArchiveObject.Items

				'--- Gets the file content and saves it to disk
				GetFileContent.renderFileToDisk(fileInObj, id, oSettings, oLogger, dirPath)

			Next

			'--- It has a Document > File structure
		ElseIf TypeOf objectResponse Is SSA.Document Then

			For Each documentInObj As SSA.Document In oGetAipResponse.ArchiveObject.Items

				'--- Avoids a null value exception if there is no file on the actual Document
				If Not documentInObj.File Is Nothing Then
					For Each fileInObj As SSA.File In documentInObj.File

						'--- Gets the file content and saves it to disk
						GetFileContent.renderFileToDisk(fileInObj, id, oSettings, oLogger, dirPath)

					Next
				End If
			Next
		End If

		'--- Just some feedback to the user
		oLogger.Log("Files saved on: " & dirPath)
	End Sub


	''' <summary>
	''' Transforms the GetAip object into an XmlDocument
	''' </summary>
	''' <param name="oGetAipResponse"></param>
	''' <param name="oSettings"></param>
	''' <param name="oLogger"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function ssaGetAipAsXml(ByVal oGetAipResponse As SSA.GetAipResponse, ByVal oSettings As Settings, ByVal oLogger As Logger) As XmlDocument
		'--- XmlDocument that will be created
		Dim xmlArchiveSip As New Xml.XmlDocument()

		'Append root node
		xmlArchiveSip.AppendChild(xmlArchiveSip.CreateElement("ArchiveSip"))
		'Prepend xml declaration
		xmlArchiveSip.InsertBefore(xmlArchiveSip.CreateXmlDeclaration("1.0", "utf-8", Nothing), xmlArchiveSip.DocumentElement)
		xmlArchiveSip.DocumentElement.SetAttribute("xmlns", "http://www.idainfront.se/schema/archive-2.2")

		'--- Creating the ArchiveObject node
		Dim xmlArchiveObject = xmlArchiveSip.CreateElement("ArchiveObject")
		xmlArchiveSip.DocumentElement.AppendChild(xmlArchiveObject)

		'--- Adding a namespace to the XML Document
		Dim xmlNSManager As New XmlNamespaceManager(xmlArchiveSip.NameTable)
		xmlNSManager.AddNamespace("arc", "http://www.idainfront.se/schema/archive-2.2")

		'----------------- Printing the ArchiveObject -----------------
		'--- Adding the DisplayName
		AddDisplayName(xmlArchiveObject, xmlArchiveSip, oGetAipResponse.ArchiveObject.DisplayName)
		'--- Adding the ObjectType
		AddObjectType(xmlArchiveObject, xmlArchiveSip, oGetAipResponse.ArchiveObject.ObjectType)

		'--- Print ArchiveAttributes
		For Each att In oGetAipResponse.ArchiveObject.Attribute
			If att.name = "lta_producer" Then 'Add the producer attribute to ArchiveSip
				xmlArchiveSip.DocumentElement.SetAttribute("producer", att.Value(0).ToString)
				FillTemplateFromAttributes(xmlArchiveObject, xmlArchiveSip, att)
			ElseIf att.name = "lta_system" Then	'Add the system attribute to ArchiveSip
				xmlArchiveSip.DocumentElement.SetAttribute("system", att.Value(0).ToString)
				FillTemplateFromAttributes(xmlArchiveObject, xmlArchiveSip, att)
			Else '--- Fills the Rest of the attributes
				FillTemplateFromAttributes(xmlArchiveObject, xmlArchiveSip, att)
			End If
		Next


		'--- Helps to know what kind of variable is going to be
		Dim objectResponse = oGetAipResponse.ArchiveObject.Items(0)

		'--- It has a File structure (No document)
		If TypeOf objectResponse Is SSA.File Then

			For Each fileInObj As SSA.File In oGetAipResponse.ArchiveObject.Items
				'--- Creates the File node
				Dim fileElement As XmlElement = createFileXmlElement(xmlArchiveSip, fileInObj)

				'--- Apends the File node to the Document node
				xmlArchiveObject.AppendChild(fileElement)
			Next

			'--- It has a Document > File structure
		ElseIf TypeOf objectResponse Is SSA.Document Then

			For Each documentInObj As SSA.Document In oGetAipResponse.ArchiveObject.Items
				'--- Creates the Document node
				Dim documentElement As XmlElement = createDocumentXmlElement(xmlArchiveSip, documentInObj)

				If Not documentInObj.File Is Nothing Then '--- Avoids a null value exception if there is no file on the actual Document
					For Each fileInObj As SSA.File In documentInObj.File
						'--- Creates the File node
						Dim fileElement As XmlElement = createFileXmlElement(xmlArchiveSip, fileInObj)

						'--- Apends the File node to the Document node
						documentElement.AppendChild(fileElement)
					Next
				End If

				'--- Appends the Document node to the ArchiveObject node
				xmlArchiveObject.AppendChild(documentElement)
			Next
		End If

		Return xmlArchiveSip
	End Function

	''' <summary>
	''' Returns the ID of the file object
	''' </summary>
	''' <param name="oFile"></param>
	''' <param name="completeIDString">If this is true it will return the complete ID String as: iipax://objectbase.document/docpartition#1231231</param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function getFileIdFromFileObject(ByVal oFile As SSA.File, Optional ByVal completeIDString As Boolean = False) As String
		For Each att In oFile.Attribute
			If att.name = "lta_file_reference" Then
				If completeIDString Then
					Return att.Value(0)
				Else
					Dim fileID As String() = att.Value(0).Split(New Char() {"#"c})
					Return fileID(1)
				End If

			End If
		Next
		Return Nothing
	End Function

End Module
