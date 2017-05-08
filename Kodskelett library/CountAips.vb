Imports System.ServiceModel

Module CountAips

	''' <summary>
	''' Returns the response of CountAips as an object
	''' </summary>
	''' <param name="oCountAips"></param>
	''' <param name="oSettings"></param>
	''' <param name="oLogger"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function ssaCountAipsObject(ByVal oCountAips As SSA.CountAips, ByVal oSettings As Settings, ByVal oLogger As Logger) As SSA.CountAipsResponse
		'--- Gets the client generated with the certificate
		Dim client As SSA.VirtualInterfaceClient = oSettings.getVirtualInterfaceClient()

		'--- The object that will be returned
		Dim oCountAipsResponse As New SSA.CountAipsResponse

		Using scope As New OperationContextScope(client.InnerChannel)

			'--- Appending the headers to the request
			oSettings.AddSOAPMessageHeaders()
			'--- Prints the endpoint settings in the logger if it has advanced logging
			oSettings.printEndpointSettings(oLogger)

			Try
				'--- Call to server
				oCountAipsResponse = client.CountAips(oCountAips)
			Catch ex As EndpointNotFoundException
				oLogger.Log("[Error] CountAips() - The endpoint is not valid.")
				If oSettings.advancedLog Then
					oLogger.Log("[Exception] - " & ex.Message.ToString)
				End If
			Catch fault As FaultException
				oLogger.Log("[Error] CountAips() - The server might not be able to process your request.")
				If oSettings.advancedLog Then
					oLogger.Log("[FaultException] - " & fault.Message.ToString)
				End If
			Catch sysEx As System.OutOfMemoryException
				oLogger.Log("[Error] CountAips() - The object you are trying to send is too large.")
				If oSettings.advancedLog Then
					oLogger.Log("[SystemException] - " & sysEx.Message.ToString)
				End If
			Catch timeEx As System.TimeoutException
				oLogger.Log("[Error] CountAips() - The time allocated for this process has expired.")
				If oSettings.advancedLog Then
					oLogger.Log("[TimeoutException] - " & timeEx.Message.ToString)
				End If
			End Try

		End Using

		client.Close()

		Return oCountAipsResponse
	End Function

End Module
