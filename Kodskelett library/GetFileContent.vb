Imports System.ServiceModel

Module GetFileContent

	''' <summary>
	''' Gets the file content and writes it to disk
	''' </summary>
	''' <param name="id"></param>
	''' <param name="oSettings"></param>
	''' <param name="oLogger"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function ssaGetFileContentResponse(ByVal id As Integer, ByVal oSettings As Settings, ByVal oLogger As Logger) As SSA.GetFileContentResponse
		'--- Gets the client generated with the certificate
		Dim client As SSA.VirtualInterfaceClient = oSettings.getVirtualInterfaceClient()

		Dim oGetFileContent As New SSA.GetFileContent
		oGetFileContent.Id = "iipax://objectbase.document/docpartition#" & id
		oGetFileContent.callerId = "?"

		'--- This is only if the advanced log is activated
		If oSettings.advancedLog Then
			oLogger.Log("[GetFileContent] Calling the server with: " & oGetFileContent.Id)
		End If

		Dim oGetFileContentResponse As New SSA.GetFileContentResponse

		Using scope As New OperationContextScope(client.InnerChannel)

			'--- Appending the headers to the request
			oSettings.AddSOAPMessageHeaders()
			'--- Prints the endpoint settings in the logger if it has advanced logging
			oSettings.printEndpointSettings(oLogger)

			Try
				'--- Call to the server
				oGetFileContentResponse = client.GetFileContent(oGetFileContent)
			Catch ex As EndpointNotFoundException
				oLogger.Log("[Error] GetFileContent(" & id & ") - The endpoint is not valid.")
				If oSettings.advancedLog Then
					oLogger.Log("[Exception] - " & ex.Message.ToString)
				End If
			Catch ex As FaultException
				oLogger.Log("[Error] GetFileContent(" & id & ") - File might be corrupted or does not exist for this object." & Environment.NewLine & ex.ToString)
				If oSettings.advancedLog Then
					oLogger.Log("[FaultException] - " & ex.Message.ToString)
				End If
			Catch sysEx As System.OutOfMemoryException
				oLogger.Log("[Error] GetFileContent(" & id & ") - The object you are trying to send is too large.")
				If oSettings.advancedLog Then
					oLogger.Log("[SystemException] - " & sysEx.Message.ToString)
				End If
			Catch timeEx As System.TimeoutException
				oLogger.Log("[Error] GetFileContent(" & id & ") - The time allocated for this process has expired.")
				If oSettings.advancedLog Then
					oLogger.Log("[TimeoutException] - " & timeEx.Message.ToString)
				End If
			End Try
		End Using

		client.Close()

		Return oGetFileContentResponse

	End Function

	''' <summary>
	''' Handles the rendering of the file into disk.
	''' </summary>
	''' <param name="fileInObj"></param>
	''' <param name="id"></param>
	''' <param name="oSettings"></param>
	''' <param name="oLogger"></param>
	''' <param name="dirPath"></param>
	''' <remarks></remarks>
	Public Sub renderFileToDisk(ByVal fileInObj As SSA.File, ByVal id As Integer, ByVal oSettings As Settings, ByVal oLogger As Logger, ByVal dirPath As String)
		'--- Parses the id from the system and only gets the numbers
		Dim fileID As String = getFileIdFromFileObject(fileInObj)
		Dim oFileContent As SSA.GetFileContentResponse = ssaGetFileContentResponse(fileID, oSettings, oLogger)

		Try
			System.IO.File.WriteAllBytes(dirPath & oFileContent.File.DisplayName, oFileContent.File.Content.Data)
			'--- Will print for every single file where has it being saved
			If oSettings.advancedLog Then
				oLogger.Log("File with ID " & id & " saved to disk on path: " & dirPath & oFileContent.File.DisplayName)
			End If

		Catch ex As Exception
			oLogger.Log("[Error on Function GetAip] - File  not witten in disk." & Environment.NewLine & ex.ToString)
		End Try
	End Sub

End Module
