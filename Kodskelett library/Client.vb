Imports System.Xml
Imports System.ServiceModel

''' <summary>
''' Sets all the settings needed to create a successfull connection to the IIPAX server.
''' </summary>
''' <remarks></remarks>
Public Class Client
	'Variable declaration
	Private _settings As Settings
	Private _logger As Logger

	''' <summary>
	''' Declares a client using the settings and a logger 
	''' </summary>
	''' <param name="oSettings">The Settings object previously declared</param>
	''' <param name="oLogger">The Logger object previously declared</param>
	''' <remarks></remarks>
	Public Sub New(ByVal oSettings As Settings, ByVal oLogger As Logger)
		Me.settings = oSettings
		Me.logger = oLogger
	End Sub


	''' <summary>
	''' Declares a client without the need of setting up a Logger, the logs will be automatically saved on logger.txt in the Project directory
	''' </summary>
	''' <param name="oSettings">The Settings object previously declared</param>
	''' <remarks></remarks>
	Public Sub New(ByVal oSettings As Settings)
		Me.settings = oSettings
		Me.logger = New Logger()
	End Sub

	''' <summary>
	''' Sets the settings that will be used with this client
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Property settings() As Settings
		Get
			Return _settings
		End Get
		Set(value As Settings)
			_settings = value
		End Set
	End Property

	''' <summary>
	''' Sets the logger that with log all the executions of the client
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Property logger() As Logger
		Get
			Return _logger
		End Get
		Set(value As Logger)
			_logger = value
		End Set
	End Property

	''' <summary>
	''' Returns the Sip object as an XML file with all the content files that corresponds to it.
	''' </summary>
	''' <param name="id">The id of the object on IIPAX</param>	
	''' <param name="dirPath">The directory path where the results will be saved to disk</param>
	''' <remarks></remarks>
	Public Sub GetAip(ByVal id As Integer, ByVal dirPath As String)
		ssaGetAip(id, Me.settings, Me.logger, dirPath)
	End Sub

	''' <summary>
	''' Returns the GetAip Sip object as a GetAip Object type
	''' </summary>
	''' <param name="id">The id of the object on IIPAX</param>	
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function GetAipAsObject(ByVal id As Integer) As SSA.GetAipResponse
		Try
			Return ssaGetAipObject(id, Me.settings, Me.logger)
		Catch ex As FaultException(Of SSA.FaultDetail)
			Me.logger.Log(ex.ToString)
			Return Nothing
		End Try
	End Function

	''' <summary>
	''' Returms the GetAip Sip object as an XmlDocument
	''' </summary>
	''' <param name="id">The id of the object on IIPAX</param>	
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function GetAipAsXml(ByVal id As Integer) As XmlDocument
		Return ssaGetAipAsXml(ssaGetAipObject(id, Me.settings, Me.logger), Me.settings, Me.logger)
	End Function

	''' <summary>
	''' Returns the file content as an object 
	''' </summary>
	''' <param name="id">The file object id on IIPAX</param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function GetFileContentAsObject(ByVal id As Integer) As SSA.File
		Return ssaGetFileContentResponse(id, Me.settings, Me.logger).File
	End Function

	''' <summary>
	''' Returns the file content as a byte array
	''' </summary>
	''' <param name="id">The file object id on IIPAX</param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function GetFileContentAsByte(ByVal id As Integer) As Byte()
		Return ssaGetFileContentResponse(id, Me.settings, Me.logger).File.Content.Data
	End Function

	''' <summary>
	''' Receives an Archive.xml and Archives it to the system
	''' </summary>
	''' <param name="archiveXmlPath">The path of the Archive.xml file</param>
	''' <remarks></remarks>
	Public Sub ArchiveSip(ByVal archiveXmlPath As String)
		ssaArchiveSip(Me.settings, Me.logger, archiveXmlPath)
	End Sub

	''' <summary>
	''' Receives a Sip object and Archives it to the system
	''' </summary>
	''' <param name="oArchiveSip">The ArchiveSip object previously declared</param>
	''' <remarks></remarks>
	Public Sub ArchiveSip(ByVal oArchiveSip As SSA.ArchiveSip)
		ssaArchiveSipObject(Me.settings, Me.logger, oArchiveSip)
	End Sub


	''' <summary>
	''' Does a GetChildren call to the server and saves the response in a generated xml file in the designated path
	''' </summary>
	''' <param name="id">The id of the object in IIPAX</param>
	''' <param name="dirPath">The path where the result will be saved</param>
	''' <remarks></remarks>
	Public Sub GetChildren(ByVal id As Integer, ByVal dirPath As String)
		ssaGetChildren(id, Me.settings, Me.logger, dirPath)
	End Sub

	''' <summary>
	''' Does a GetChildren call to the server and saves the response in a generated xml file in the designated path and the file name
	''' </summary>
	''' <param name="id">The id of the object in IIPAX</param>
	''' <param name="dirPath">The path where the result will be saved</param>
	''' <param name="fileName">The file name that will contain the results</param>
	''' <remarks></remarks>
	Public Sub GetChildren(ByVal id As Integer, ByVal dirPath As String, ByVal fileName As String)
		ssaGetChildren(id, Me.settings, Me.logger, dirPath, fileName)
	End Sub


	''' <summary>
	''' Does a GetChildren call to the server and returns the response as an object array
	''' </summary>
	''' <param name="id">The id of the object in IIPAX</param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function GetChildrenAsObject(ByVal id As Integer) As SSA.Object()
		Return ssaGetChildrenObject(id, Me.settings, Me.logger)
	End Function

	''' <summary>
	''' Returns the an array of Sip objects given the type of Search
	''' </summary>
	''' <param name="oSearchAip"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function SearchAipsAsObject(ByVal oSearchAip As SSA.SearchAips) As SSA.ArchiveObject()
		Return ssaSearchAipsObject(oSearchAip, Me.settings, Me.logger).ArchiveObject
	End Function

	''' <summary>
	''' Returns the SearchAipsResponse as an object.
	''' </summary>
	''' <param name="oSearchAip">This object can be generated using the SSA.SearchAips reference</param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function SearchAipsAsObjectResponse(ByVal oSearchAip As SSA.SearchAips) As SSA.SearchAipsResponse
		Return ssaSearchAipsObject(oSearchAip, Me.settings, Me.logger)
	End Function

	''' <summary>
	''' Returns the number of the amount of Sip objects in the system according to the parameters given
	''' </summary>
	''' <param name="oCountAip">The CountAips object that has been previously declared</param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function CountAips(ByVal oCountAip As SSA.CountAips) As Long
		Return ssaCountAipsObject(oCountAip, Me.settings, Me.logger).Number
	End Function

	''' <summary>
	''' Returns the CountAipsResponse as an object
	''' </summary>
	''' <param name="oCountAip">The CountAips object that has been previously declared</param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function CountAipsAsObjectResponse(ByVal oCountAip As SSA.CountAips) As SSA.CountAipsResponse
		Return ssaCountAipsObject(oCountAip, Me.settings, Me.logger)
	End Function
End Class