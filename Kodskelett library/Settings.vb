Imports System.Security.Cryptography.X509Certificates
Imports System.ServiceModel

Public Class Settings
	'Variable declaration
	Private _certificatePath As String
	Private _certificatePassword As String
	Private _endpoint As String
	Private _httpIp As String
	Private _httpRole As String
	Private _maxReceivedMessageSize As Long
	Private _maxBufferPoolSize As Long
	Private _closeTimeout As TimeSpan
	Private _openTimeout As TimeSpan
	Private _receiveTimeout As TimeSpan
	Private _sendTimeout As TimeSpan
	Private _messageEncoding As System.ServiceModel.WSMessageEncoding
	Private _securityMode As System.ServiceModel.SecurityMode
	Private _clientCredentialType As System.ServiceModel.HttpClientCredentialType
	Private _advancedLog As Boolean

	''' <summary>
	''' Declares the settings object with all the needed parameters in order to work
	''' </summary>
	''' <param name="strEndpoint">The endpoint where it will be connecting</param>
	''' <param name="strCertificatePath">The path of the certificate .pfx</param>
	''' <param name="strCertificatePassword">The password of the certificate</param>
	''' <param name="strHttpIp">Attribute HTTP_IP that will be appended to the headers of the request</param>
	''' <param name="strHttpRole">Attribute HTTP_ROLE that will be appended to the headers of the request</param>
	''' <param name="advancedLog">Warning: Do not activate this if you are planning to execute multiple server calls, this might generate a huge Log file and slow the process.</param>
	''' <remarks></remarks>
	Public Sub New(ByVal strEndpoint As String, ByVal strCertificatePath As String, ByVal strCertificatePassword As String, ByVal strHttpIp As String, Optional ByVal strHttpRole As String = "Master.All", Optional ByVal advancedLog As Boolean = False)
		Me.endpoint = strEndpoint
		Me.certificatePath = strCertificatePath
		Me.certificatePassword = strCertificatePassword
		Me.httpIp = strHttpIp
		Me.httpRole = strHttpRole
		Me.advancedLog = advancedLog
	End Sub

	''' <summary>
	''' Gets or sets the Endpoint where the connection will be done
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Property endpoint() As String
		Get
			Return _endpoint
		End Get
		Set(value As String)
			_endpoint = value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the certificate path
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Property certificatePath() As String
		Get
			Return _certificatePath
		End Get
		Set(value As String)
			_certificatePath = value
		End Set
	End Property


	''' <summary>
	''' Gets or sets the password that the certificate requires in order to work
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Property certificatePassword() As String
		Get
			Return _certificatePassword
		End Get
		Set(value As String)
			_certificatePassword = value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the HttpRole that will be appended to the headers of the request
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Property httpIp() As String
		Get
			Return _httpIp
		End Get
		Set(value As String)
			_httpIp = value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the HttpRole that will be appended to the headers of the request
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Property httpRole() As String
		Get
			Return _httpRole
		End Get
		Set(value As String)
			_httpRole = value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the Advanced logging feature
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Property advancedLog() As Boolean
		Get
			Return _advancedLog
		End Get
		Set(value As Boolean)
			_advancedLog = value
		End Set
	End Property


	''' <summary>
	''' Gets and sets the MaxReceivedMessageSize attribute on the Virtual interface client
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property maxReceivedMessageSize() As Long
		Get
			If _maxReceivedMessageSize = Nothing Then
				Return 20000000
			Else
				Return _maxReceivedMessageSize
			End If
		End Get
		Set(ByVal Value As Long)
			_maxReceivedMessageSize = Value
		End Set
	End Property

	''' <summary>
	''' Gets and sets the MaxBufferPoolSize attribute on the Virtual interface client
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property maxBufferPoolSize() As Long
		Get
			If _maxBufferPoolSize = Nothing Then
				Return 20000000
			Else
				Return _maxBufferPoolSize
			End If
		End Get
		Set(ByVal Value As Long)
			_maxBufferPoolSize = Value
		End Set
	End Property

	''' <summary>
	''' Gets and sets the closeTimeout attribute on the Virtual interface client
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property closeTimeout() As TimeSpan
		Get
			If _closeTimeout = Nothing Then
				Return New TimeSpan(0, 25, 0)
			Else
				Return _closeTimeout
			End If
		End Get
		Set(ByVal Value As TimeSpan)
			_closeTimeout = Value
		End Set
	End Property

	''' <summary>
	''' Gets and sets the openTimeout attribute on the Virtual interface client
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property openTimeout() As TimeSpan
		Get
			If _openTimeout = Nothing Then
				Return New TimeSpan(0, 20, 0)
			Else
				Return _openTimeout
			End If
		End Get
		Set(ByVal Value As TimeSpan)
			_openTimeout = Value
		End Set
	End Property

	''' <summary>
	''' Gets and sets the receiveTimeout attribute on the Virtual interface client
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property receiveTimeout() As TimeSpan
		Get
			If _receiveTimeout = Nothing Then
				Return New TimeSpan(0, 10, 0)
			Else
				Return _receiveTimeout
			End If
		End Get
		Set(ByVal Value As TimeSpan)
			_receiveTimeout = Value
		End Set
	End Property

	''' <summary>
	''' Gets and sets the sendTimeout attribute on the Virtual interface client
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property sendTimeout() As TimeSpan
		Get
			If _sendTimeout = Nothing Then
				Return New TimeSpan(0, 25, 0)
			Else
				Return _sendTimeout
			End If
		End Get
		Set(ByVal Value As TimeSpan)
			_sendTimeout = Value
		End Set
	End Property

	''' <summary>
	''' Gets and sets the MessageEncoding attribute on the Virtual interface client
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property messageEncoding() As System.ServiceModel.WSMessageEncoding
		Get
			If _messageEncoding = Nothing Then
				Return ServiceModel.WSMessageEncoding.Mtom
			Else
				Return _messageEncoding
			End If
		End Get
		Set(ByVal Value As System.ServiceModel.WSMessageEncoding)
			_messageEncoding = Value
		End Set
	End Property

	''' <summary>
	''' Gets and sets the Security Mode attribute on the Virtual interface client
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property securityMode() As System.ServiceModel.SecurityMode
		Get
			If _securityMode = Nothing Then
				Return ServiceModel.SecurityMode.Transport
			Else
				Return _securityMode
			End If
		End Get
		Set(ByVal Value As System.ServiceModel.SecurityMode)
			_securityMode = Value
		End Set
	End Property



	''' <summary>
	''' Gets and sets the Security Mode attribute on the Virtual interface client
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property clientCredentialType() As System.ServiceModel.HttpClientCredentialType
		Get
			If _clientCredentialType = Nothing Then
				Return ServiceModel.HttpClientCredentialType.Certificate
			Else
				Return _clientCredentialType
			End If
		End Get
		Set(ByVal Value As System.ServiceModel.HttpClientCredentialType)
			_clientCredentialType = Value
		End Set
	End Property

	''' <summary>
	''' Returns the virtual Interface client for the Endpoint with the certificate appended
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	ReadOnly Property getVirtualInterfaceClient() As SSA.VirtualInterfaceClient
		Get
			'--- Creating a new TempClient, you can't reuse a client session for nested calls
			Dim cert As New X509Certificate2(Me.certificatePath, Me.certificatePassword)

			Dim httpBinding As New ServiceModel.WSHttpBinding '--- Maybe(?) Missing adding the receive,send,open,close timeouts
			'--- Increasing the message size quota
			httpBinding.MaxReceivedMessageSize = Me.maxReceivedMessageSize
			httpBinding.MaxBufferPoolSize = Me.maxBufferPoolSize
			'--- The timeouts
			httpBinding.CloseTimeout = Me.closeTimeout
			httpBinding.OpenTimeout = Me.openTimeout
			httpBinding.ReceiveTimeout = Me.receiveTimeout
			httpBinding.SendTimeout = Me.sendTimeout
			'--- Adding Mtom as the Message encoding 
			httpBinding.MessageEncoding = Me.messageEncoding
			'--- This attribute needs to be there
			httpBinding.Security.Mode = Me.securityMode
			'--- Indicating that it will use a certificate
			httpBinding.Security.Transport.ClientCredentialType = Me.clientCredentialType
			'--- Sets the endpoint address with the one declared on this instance of object
			Dim endpointAddress As New ServiceModel.EndpointAddress(Me.endpoint)
			'--- Creates the virtual Interface
			Dim virtualInterface As New SSA.VirtualInterfaceClient(httpBinding, endpointAddress)

			virtualInterface.ClientCredentials.ClientCertificate.Certificate = cert

			Return virtualInterface

		End Get
	End Property

	''' <summary>
	''' This is done only if there is advanced logging activated
	''' </summary>
	''' <remarks></remarks>
	Public Sub printEndpointSettings(ByVal oLogger As Logger)
		If Me.advancedLog Then
			oLogger.Log("Using certificate located in: " & Me.certificatePath & ". Certificate password: " & Me.certificatePassword)
			oLogger.Log("--- HttpBinding settings ---")
			oLogger.Log("Endpoint = " & Me.endpoint)
			oLogger.Log("MaxReceivedMessageSize = " & Me.maxReceivedMessageSize)
			oLogger.Log("MaxBufferPoolSize = " & Me.maxBufferPoolSize)
			oLogger.Log("CloseTimeout = " & Me.closeTimeout.ToString("hh\:mm\:ss"))
			oLogger.Log("OpenTimeout = " & Me.openTimeout.ToString("hh\:mm\:ss"))
			oLogger.Log("ReceiveTimeout = " & Me.receiveTimeout.ToString("hh\:mm\:ss"))
			oLogger.Log("SendTimeout = " & Me.sendTimeout.ToString("hh\:mm\:ss"))
			oLogger.Log("MessageEncoding = " & Me.messageEncoding.ToString)
			oLogger.Log("SecurityMode = " & Me.securityMode.ToString)
			oLogger.Log("ClientCredentialType = " & Me.clientCredentialType.ToString)
			oLogger.Log("----------------------------")
		End If
	End Sub

	''' <summary>
	''' Method to append the SOAP headers
	''' </summary>	
	''' <remarks></remarks>
	Public Sub AddSOAPMessageHeaders()
		Dim mProperty = New Channels.HttpRequestMessageProperty
		mProperty.Headers.Add("HTTP_ROLE", Me.httpRole)
		mProperty.Headers.Add("HTTP_IP", Me.httpIp)

		OperationContext.Current.OutgoingMessageProperties(Channels.HttpRequestMessageProperty.Name) = mProperty
	End Sub

End Class
