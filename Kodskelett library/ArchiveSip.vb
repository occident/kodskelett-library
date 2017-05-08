Imports System.ServiceModel
Imports System.Xml
Imports System.IO
Imports System.Security.Cryptography

Module ArchiveSip

	''' <summary>
	''' Makes a the connection with the system and sends an Archive.xml
	''' </summary>
	''' <param name="oSettings"></param>
	''' <param name="oLogger"></param>
	''' <param name="documentPath"></param>
	''' <remarks></remarks>
	Public Sub ssaArchiveSip(ByVal oSettings As Settings, ByVal oLogger As Logger, ByVal documentPath As String)
		'--- Makes the connection to the server
		ssaArchiveSipObject(oSettings, oLogger, xmlToArchiveSip(oLogger, documentPath))
	End Sub


	''' <summary>
	''' Makes a the connection with the system and sends a Sip object
	''' </summary>
	''' <param name="oSettings"></param>
	''' <param name="oLogger"></param>
	''' <param name="oArchiveSip"></param>
	''' <remarks></remarks>
	Public Sub ssaArchiveSipObject(ByVal oSettings As Settings, ByVal oLogger As Logger, ByVal oArchiveSip As SSA.ArchiveSip)
		'--- Gets the client generated with the certificate
		Dim client As SSA.VirtualInterfaceClient = oSettings.getVirtualInterfaceClient()

		'---- Creates the connection to send the ArchiveObject
		Using scope As New OperationContextScope(client.InnerChannel)
			'--- Appending the headers to the request
			oSettings.AddSOAPMessageHeaders()
			'--- Prints the endpoint settings in the logger if it has advanced logging
			oSettings.printEndpointSettings(oLogger)

			Try
				Dim oArchiveSipResponse As SSA.ArchiveSipResponse = client.ArchiveSip(oArchiveSip)

				oLogger.Log("Archived Successfully with new ID: " & oArchiveSipResponse.Id)
			Catch ex As EndpointNotFoundException
				oLogger.Log("[Error] ArchiveSip(" & oArchiveSip.ArchiveObject.DisplayName & ") - The endpoint is not valid.")
				If oSettings.advancedLog Then
					oLogger.Log("[Exception] - " & ex.Message.ToString)
				End If
			Catch fault As FaultException
				oLogger.Log("[Error - FaultException] ArchiveSip(" & oArchiveSip.ArchiveObject.DisplayName & ") - " & fault.Message.ToString)
			Catch sysEx As System.OutOfMemoryException
				oLogger.Log("[Error] ArchiveSip(" & oArchiveSip.ArchiveObject.DisplayName & ") - The object you are trying to send is too large.")
				If oSettings.advancedLog Then
					oLogger.Log("[SystemException] - " & sysEx.Message.ToString)
				End If
			Catch timeEx As System.TimeoutException
				oLogger.Log("[Error] ArchiveSip(" & oArchiveSip.ArchiveObject.DisplayName & ") - The time allocated for this process has expired.")
				If oSettings.advancedLog Then
					oLogger.Log("[TimeoutException] - " & timeEx.Message.ToString)
				End If
			End Try

		End Using

		client.Close()
	End Sub

	''' <summary>
	''' This is supposed to be a much better way to do the ArchiveSip when it needs to handle objects like "File" or "Document"
	''' </summary>
	''' <param name="oLogger"></param>
	''' <param name="documentPath"></param>
	''' <remarks></remarks>
	Public Function xmlToArchiveSip(ByVal oLogger As Logger, ByVal documentPath As String) As SSA.ArchiveSip
		'--- Variable declaration
		Dim oArchiveSip As New SSA.ArchiveSip

		'--- ArchiveObject Variables
		Dim oArchiveObject As New SSA.ArchiveObject
		Dim oArchiveItems As New List(Of SSA.Document)
		Dim oFileItems As New List(Of SSA.File)
		Dim oArchiveObjectAttributeList As New List(Of SSA.NameValue)

		'--- Load the XML file to Memory
		Dim inputXmlDocument As XmlDocument = New XmlDocument()
		inputXmlDocument.Load(documentPath)

		Dim xArchiveSip As XmlElement = inputXmlDocument.GetElementsByTagName("ArchiveSip").Item(0)	'--- Gets the ArchiveSip Node
		Dim xArchiveObject As XmlElement = xArchiveSip.ChildNodes.Item(0) '--- Gets the ArchiveObject node

		Dim xArchiveObjectChild As XmlNodeList = xArchiveObject.ChildNodes

		'--- oDocument variables
		Dim oDocument As SSA.Document
		Dim oDocumentAttributeList As New List(Of SSA.NameValue)

		For Each childArchiveObject As XmlNode In xArchiveObjectChild
			'--- Build the oArchiveObject
			If childArchiveObject.Name = "DisplayName" Then
				'--- Sets the DisplayName to oArchiveObject
				oArchiveObject.DisplayName = childArchiveObject.InnerText
			ElseIf childArchiveObject.Name = "ObjectType" Then
				'--- Sets the ObjectType to oArchiveObject
				oArchiveObject.ObjectType = childArchiveObject.InnerText
			ElseIf childArchiveObject.Name = "Attribute" Then

				'--- For some reason it wants an array...
				Dim newFakeStringArray(0) As String
				newFakeStringArray(0) = childArchiveObject.InnerText

				Dim oArchiveObjectAttribute As New SSA.NameValue
				oArchiveObjectAttribute.name = childArchiveObject.Attributes(0).Value
				oArchiveObjectAttribute.Value = newFakeStringArray

				oArchiveObjectAttributeList.Add(oArchiveObjectAttribute)
			End If

			oDocumentAttributeList = New List(Of SSA.NameValue)

			'--- Check if it has childs (Items)
			'--- It seems that there's a lot of code repeated here, but this helps to know which type objects are going to be created, since it changes if it includes a file or not
			If childArchiveObject.Name = "Document" Then '--- If it is a Document with a nested File node
				oFileItems = New List(Of SSA.File)
				oDocument = New SSA.Document

				Dim xDocumentChild As XmlNodeList = childArchiveObject.ChildNodes
				For Each childDocument As XmlNode In xDocumentChild
					'---Build the oDocument
					If childDocument.Name = "DisplayName" Then
						'--- Sets the DisplayName to oDocument
						oDocument.DisplayName = childDocument.InnerText
					ElseIf childDocument.Name = "ObjectType" Then
						'--- Sets the ObjectType to oDocument
						oDocument.ObjectType = childDocument.InnerText
					ElseIf childDocument.Name = "Attribute" Then

						'--- For some reason it wants an array...
						Dim newFakeStringArray(0) As String
						newFakeStringArray(0) = childDocument.InnerText

						Dim oDocumentAttribute As New SSA.NameValue
						oDocumentAttribute.name = childDocument.Attributes(0).Value
						oDocumentAttribute.Value = newFakeStringArray

						oDocumentAttributeList.Add(oDocumentAttribute)

					ElseIf childDocument.Name = "File" Then
						'--- Include the File Object to the oFileItems list
						oFileItems.Add(fileHandler(oLogger, childDocument, documentPath))
					End If

				Next

				'--- Sets the Attributes to oDocument
				oDocument.Attribute = oDocumentAttributeList.ToArray()

				'--- Add all the files to the oDocument
				oDocument.File = oFileItems.ToArray

				'--- Adding the document node to the oArchiveItems array
				oArchiveItems.Add(oDocument)


			ElseIf childArchiveObject.Name = "File" Then
				'--- Saves the items of the file
				oFileItems.Add(fileHandler(oLogger, childArchiveObject, documentPath))
			End If
		Next

		'--- Sets the Attributes to oArchiveObject
		oArchiveObject.Attribute = oArchiveObjectAttributeList.ToArray

		'--- Finish filling out the ArchiveObject items
		'--- If there was a first level node with file Ex: ArchiveObject > File > Content
		If oArchiveItems.ToArray.Count = 0 Then
			oArchiveObject.Items = oFileItems.ToArray
		Else '--- If there was a second level node with File EX: ArchiveObject > Document > File > Content
			oArchiveObject.Items = oArchiveItems.ToArray
		End If


		'--- ArchiveSip Object declaration
		oArchiveSip.callerId = "?" '--- I don't know if this is mandatory or not
		oArchiveSip.producer = xArchiveSip.GetAttribute("producer")
		oArchiveSip.system = xArchiveSip.GetAttribute("system")
		oArchiveSip.ArchiveObject = oArchiveObject

		Return oArchiveSip

	End Function

	''' <summary>
	''' Extracts from an XML the values that the File Object needs
	''' </summary>
	''' <param name="oLogger"></param>
	''' <param name="childDocument"></param>
	''' <param name="documentPath"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function fileHandler(ByVal oLogger As Logger, ByVal childDocument As XmlNode, ByVal documentPath As String) As SSA.File
		Dim oFile As New SSA.File
		Dim oContent As New SSA.Content
		Dim digest As String = ""
		Dim strFileLength As String = ""
		Dim oFileAttributeList As New List(Of SSA.NameValue)

		For Each childFile As XmlNode In childDocument
			'oLogger.Log(vbTab & vbTab & childFile.Name)
			'---Build the oFile

			If childFile.Name = "DisplayName" Then
				'--- Sets the DisplayName to oFile
				oFile.DisplayName = childFile.InnerText
			ElseIf childFile.Name = "ObjectType" Then
				'--- Sets the ObjectType to oFile
				oFile.ObjectType = childFile.InnerText
			ElseIf childFile.Name = "Attribute" Then

				'--- For some reason it wants an array...
				Dim newFakeStringArray(0) As String
				newFakeStringArray(0) = childFile.InnerText

				Dim oFileAttribute As New SSA.NameValue
				oFileAttribute.name = childFile.Attributes(0).Value
				oFileAttribute.Value = newFakeStringArray

				oFileAttributeList.Add(oFileAttribute)

				If childFile.Attributes(0).Value = "lta_file_digest" Then
					digest = childFile.FirstChild.InnerText
				End If

			ElseIf childFile.Name = "Content" Then
				Dim filePath As String = System.IO.Path.GetDirectoryName(documentPath) & "\" & childFile.FirstChild.InnerText

				'--- This is handled by a function since it helps to have better control of the memory used by the machine executing the script
				oContent.Data = FileToByteArray(oLogger, filePath)
				'--- Adds the digest
				oContent.digest = digest

			End If

			'--- Appends the oContent object to the oFile
			oFile.Content = oContent
		Next

		'--- Sets the Attributes to oFile
		oFile.Attribute = oFileAttributeList.ToArray

		'--- Include the File Object to the oFileItems list
		Return oFile

	End Function


	''' <summary>
	''' Get a SHA1 checksum for a given file
	''' </summary>
	''' <param name="filePath">The path where the file is located</param>
	''' <param name="oLogger">The file that is being used for logging</param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function GetSha1Hash(ByVal filePath As String, ByVal oLogger As Logger) As String
		Dim shaResult As String = ""
		Try
			Using fs = File.OpenRead(filePath)
				Dim sha = New SHA1Managed()
				shaResult = "SHA1:" & BitConverter.ToString(sha.ComputeHash(fs)).Replace("-", "").ToLower
			End Using
		Catch ex As Exception
			oLogger.Log("ERROR - File missing when generating Sha1Hash: " & filePath)
		End Try

		Return shaResult

	End Function

	''' <summary>
	''' Returns the byte of an array on a file
	''' </summary>
	''' <param name="oLogger"></param>
	''' <param name="_FileName"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function FileToByteArray(ByVal oLogger As Logger, ByVal _FileName As String) As Byte()
		Dim _Buffer() As Byte = Nothing
		Try
			' Open file for reading
			Dim _FileStream As New System.IO.FileStream(_FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read)

			' attach filestream to binary reader
			Dim _BinaryReader As New System.IO.BinaryReader(_FileStream)
			' get total byte length of the file
			Dim _TotalBytes As Long = New System.IO.FileInfo(_FileName).Length

			' read entire file into buffer
			_Buffer = _BinaryReader.ReadBytes(CInt(Fix(_TotalBytes)))

			' close file reader
			_FileStream.Close()
			_FileStream.Dispose()
			_BinaryReader.Close()
		Catch _Exception As Exception
			' Error
			oLogger.Log("Exception caught in process of converting the file to an Array: ", _Exception.ToString())
		End Try
		Return _Buffer
	End Function
End Module
