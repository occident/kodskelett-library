Imports System.ServiceModel
Imports System.Xml

Module GetChildren

	''' <summary>
	''' Gets the children of an ArchiveSip as an object , For example: "File" or "Document" nodes.
	''' </summary>
	''' <param name="id"></param>
	''' <param name="oSettings"></param>
	''' <param name="oLogger"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function ssaGetChildrenObject(ByVal id As Integer, ByVal oSettings As Settings, ByVal oLogger As Logger) As SSA.Object()
		'--- Gets the client generated with the certificate
		Dim client As SSA.VirtualInterfaceClient = oSettings.getVirtualInterfaceClient()
		'--- Where the response will be saved
		Dim oGetChildrenResponse() As SSA.Object = Nothing

		Dim oGetChildren As New SSA.GetChildren
		oGetChildren.Id = "iipax://objectbase.document/docpartition#" & id
		oGetChildren.callerId = "?"


		'--- This is only if the advanced log is activated
		If oSettings.advancedLog Then
			oLogger.Log("[GetChildren] Calling the server with: " & oGetChildren.Id)
		End If


		Using scope As New OperationContextScope(client.InnerChannel)

			'--- Appending the headers to the request
			oSettings.AddSOAPMessageHeaders()
			'--- Prints the endpoint settings in the logger if it has advanced logging
			oSettings.printEndpointSettings(oLogger)

			Try
				'--- Object that stores the ArchiveSip Object response from the server
				oGetChildrenResponse = client.GetChildren(oGetChildren)

				'--- Logging sucessfull message
				oLogger.Log("GetChildren(" & id & ") - Status: Success.")
			Catch ex As EndpointNotFoundException
				oLogger.Log("[Error] GetChildren(" & id & ") - The endpoint is not valid.")
				If oSettings.advancedLog Then
					oLogger.Log("[Exception] - " & ex.Message.ToString)
				End If
			Catch fault As FaultException
				oLogger.Log("[Error] GetChildren(" & id & ") - The ID might not be correct or does not exist on the system.")
				If oSettings.advancedLog Then
					oLogger.Log("[FaultException] - " & fault.Message.ToString)
				End If
			Catch sysEx As System.OutOfMemoryException
				oLogger.Log("[Error] GetChildren(" & id & ") - The object you are trying to send is too large.")
				If oSettings.advancedLog Then
					oLogger.Log("[SystemException] - " & sysEx.Message.ToString)
				End If
			Catch timeEx As System.TimeoutException
				oLogger.Log("[Error] GetChildren(" & id & ") - The time allocated for this process has expired.")
				If oSettings.advancedLog Then
					oLogger.Log("[TimeoutException] - " & timeEx.Message.ToString)
				End If
			End Try
		End Using

		client.Close()

		Return oGetChildrenResponse

	End Function

	''' <summary>
	''' Gets the children of an ArchiveSip, For example: "File" or "Document" nodes.
	''' </summary>
	''' <param name="id"></param>
	''' <param name="oSettings"></param>
	''' <param name="oLogger"></param>
	''' <param name="dirPath"></param>
	''' <param name="fileName"></param>
	''' <remarks></remarks>
	Public Sub ssaGetChildren(ByVal id As Integer, ByVal oSettings As Settings, ByVal oLogger As Logger, ByVal dirPath As String, Optional ByVal fileName As String = "")
		'--- Call to server
		Dim oGetChildrenResponse() As SSA.Object = ssaGetChildrenObject(id, oSettings, oLogger)

		'--- Creates the output directory if it does not exist
		If Not IO.File.Exists(dirPath) Then
			IO.Directory.CreateDirectory(dirPath)
		End If

		Dim tempFileName As String = id & "_children.xml"

		'--- Checks if there is a custom file name
		If fileName <> "" Then
			tempFileName = fileName & ".xml"
		End If

		'--- Some advanced Logging
		If oSettings.advancedLog Then
			oLogger.Log("Generating Archive.xml from object...")
		End If
		'--- Saves to disk 
		objectToXml(oGetChildrenResponse).Save(IO.Path.Combine(dirPath, tempFileName))
		If oSettings.advancedLog Then
			oLogger.Log("File saved to disk on path: " & "\" & tempFileName)
		End If

	End Sub


	''' <summary>
	''' Converts an SSA.Object array in a XmlDocument
	''' </summary>
	''' <param name="oObject"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function objectToXml(ByVal oObject() As SSA.Object) As XmlDocument
		Dim xmlGetChildrenResponse As New Xml.XmlDocument()	'--- XmlDocument that will be created

		'Append root node
		xmlGetChildrenResponse.AppendChild(xmlGetChildrenResponse.CreateElement("GetChildrenResponse"))
		'Prepend xml declaration
		xmlGetChildrenResponse.InsertBefore(xmlGetChildrenResponse.CreateXmlDeclaration("1.0", "utf-8", Nothing), xmlGetChildrenResponse.DocumentElement)
		xmlGetChildrenResponse.DocumentElement.SetAttribute("xmlns", "http://www.idainfront.se/schema/archive-2.2")

		'--- Adding a namespace to the XML Document
		Dim xmlNSManager As New XmlNamespaceManager(xmlGetChildrenResponse.NameTable)
		xmlNSManager.AddNamespace("arc", "http://www.idainfront.se/schema/archive-2.2")

		If oObject.Count > 0 Then
			For Each singleObject As SSA.Object In oObject
				'--- Creating the Object node
				Dim xmlObject As XmlElement
				xmlObject = xmlGetChildrenResponse.CreateElement("Object")

				'--- Adding the DisplayName
				AddDisplayName(xmlObject, xmlGetChildrenResponse, singleObject.DisplayName)
				'--- Adding the ObjectType
				AddObjectType(xmlObject, xmlGetChildrenResponse, singleObject.ObjectType)
				'--- Fills the Attributes of the Node
				AddAttributes(xmlObject, xmlGetChildrenResponse, singleObject.Attribute)

				'--- Appends the Object node to the Parent
				xmlGetChildrenResponse.DocumentElement.AppendChild(xmlObject)
			Next
		End If


		Return xmlGetChildrenResponse

	End Function
End Module
