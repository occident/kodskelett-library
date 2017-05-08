Imports System.IO

''' <summary>
''' Service class for logging messages to specific file. 
''' </summary>
''' <remarks></remarks>
Public Class Logger
	Private writer As StreamWriter

	''' <summary>
	''' Instanties the render log without any attributes. This will generate a logger.txt in the Project main folder
	''' </summary>
	''' <remarks></remarks>
	Public Sub New()
		'--- Getting the main folder directory
		Dim mainDirectory = AppDomain.CurrentDomain.BaseDirectory.Replace("\bin\Debug\", "")
		Dim strPath As String = mainDirectory & "\logger.txt"
		If Not IO.File.Exists(strPath) Then
			writer = IO.File.CreateText(strPath)
		Else
			writer = New StreamWriter(strPath)
		End If

	End Sub


	''' <summary>
	''' Instantiates the render log. Will create the necessary directory and file if they don't yet exist
	''' </summary>
	''' <param name="strDir">The path where it will be saved</param>
	''' <param name="logName">The name of the file</param>
	''' <remarks></remarks>
	Public Sub New(ByVal strDir As String, ByVal logName As String)
		'Dim strDir As String = Environment.CurrentDirectory & "\Utdata"
		Dim strPath As String = strDir & "\" & logName
		If Not IO.Directory.Exists(strDir) Then
			IO.Directory.CreateDirectory(strDir)
		End If
		If Not IO.File.Exists(strPath) Then
			writer = IO.File.CreateText(strPath)
		Else
			writer = New StreamWriter(strPath)
		End If
	End Sub

	''' <summary>
	''' Sets the logger to write to this stream
	''' </summary>
	''' <param name="oWriter"></param>
	''' <remarks></remarks>
	Public Sub New(ByVal oWriter As StreamWriter)
		Me.writer = oWriter
	End Sub

	''' <summary>
	''' Logs the message to a new line
	''' </summary>
	''' <param name="strLogMessage"></param>
	''' <remarks></remarks>
	Public Sub Log(ByVal strLogMessage As String)
		writer.WriteLine(strLogMessage)
		writer.Flush()
	End Sub

	Public Function GetStream() As IO.Stream
		Return writer.BaseStream
	End Function

	''' <summary>
	''' Shorthand for logging using <see cref="String.Format"></see>
	''' </summary>
	''' <param name="strLogBase"></param>
	''' <param name="formatParams"></param>
	''' <remarks></remarks>
	Public Sub Log(ByVal strLogBase As String, ByVal ParamArray formatParams() As Object)
		writer.WriteLine(String.Format(strLogBase, formatParams))
	End Sub

	''' <summary>
	''' Flushes everything and closes the underlying <see cref="IO.StreamWriter"></see>
	''' </summary>
	''' <remarks></remarks>
	Public Sub Close()
		writer.Flush()
		writer.Close()
	End Sub
End Class
