Imports System.ServiceModel

Module SearchAips
	''' <summary>
	''' Returns a Response that include Sip objects.
	''' </summary>
	''' <param name="oSearchAip"></param>
	''' <param name="oSettings"></param>
	''' <param name="oLogger"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function ssaSearchAipsObject(ByVal oSearchAip As SSA.SearchAips, ByVal oSettings As Settings, ByVal oLogger As Logger) As SSA.SearchAipsResponse
		'--- Gets the client generated with the certificate
		Dim client As SSA.VirtualInterfaceClient = oSettings.getVirtualInterfaceClient()

		'--- The object that will be returned
		Dim oSearchAipResponse As New SSA.SearchAipsResponse

		Using scope As New OperationContextScope(client.InnerChannel)

			'--- Appending the headers to the request
			oSettings.AddSOAPMessageHeaders()
			'--- Prints the endpoint settings in the logger if it has advanced logging
			oSettings.printEndpointSettings(oLogger)

			Try
				'--- Call to server
				oSearchAipResponse = client.SearchAips(oSearchAip)

			Catch ex As EndpointNotFoundException
				oLogger.Log("[Error] SearchAips() - The endpoint is not valid.")
				If oSettings.advancedLog Then
					oLogger.Log("[Exception] - " & ex.Message.ToString)
				End If
			Catch fault As FaultException
				oLogger.Log("[Error] SearchAips() - The server might not be able to process your request.")
				If oSettings.advancedLog Then
					oLogger.Log("[FaultException] - " & fault.Message.ToString)
				End If
			Catch sysEx As System.OutOfMemoryException
				oLogger.Log("[Error] SearchAips() - The object you are trying to send is too large.")
				If oSettings.advancedLog Then
					oLogger.Log("[SystemException] - " & sysEx.Message.ToString)
				End If
			Catch timeEx As System.TimeoutException
				oLogger.Log("[Error] SearchAips() - The time allocated for this process has expired.")
				If oSettings.advancedLog Then
					oLogger.Log("[TimeoutException] - " & timeEx.Message.ToString)
				End If
			End Try
		End Using

		client.Close()

		Return oSearchAipResponse
	End Function
End Module
