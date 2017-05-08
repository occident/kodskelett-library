Imports System.Xml

''' <summary>
''' This Module is intented to help with some functions to be able to handle Xml elements in an easier way 
''' </summary>
''' <remarks></remarks>
Module xmlWriter
	''' <summary>
	''' Creates a child object xml template and adds a DisplayName tag to it
	''' </summary>
	''' <param name="xmlDocTemplate"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function GetXMLChildObject(ByVal xmlDocTemplate As Xml.XmlDocument) As Xml.XmlElement
		Dim xmlChildTemplate = xmlDocTemplate.CreateElement("Document")
		Dim tmpNode = xmlDocTemplate.CreateElement("DisplayName")
		xmlChildTemplate.AppendChild(tmpNode)
		Return xmlChildTemplate
	End Function

	''' <summary>
	''' Creates a child object xml template for a File and adds a DisplayName tag to it
	''' </summary>
	''' <param name="xmlDocTemplate"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function GetXMLChildFileObject(ByVal xmlDocTemplate As Xml.XmlDocument) As Xml.XmlElement
		Dim xmlChildTemplate = xmlDocTemplate.CreateElement("File")
		Dim tmpNode = xmlDocTemplate.CreateElement("DisplayName")
		xmlChildTemplate.AppendChild(tmpNode)
		Return xmlChildTemplate
	End Function

	''' <summary>
	''' Adds an object type tag to the <paramref name="xmlTemplate"></paramref>, with the value of the given <paramref name="oCell">cell</paramref>
	''' </summary>
	''' <param name="xmlTemplate"></param>
	''' <param name="xmlDocTemplate"></param>
	''' <param name="oValue"></param>
	''' <remarks></remarks>
	Sub AddDisplayName(ByVal xmlTemplate As XmlElement, ByVal xmlDocTemplate As XmlDocument, ByVal oValue As String)
		Dim xmlObjectType = xmlDocTemplate.CreateElement("DisplayName")
		xmlObjectType.InnerText = oValue
		xmlTemplate.AppendChild(xmlObjectType)
	End Sub

	''' <summary>
	''' Adds an object type tag to the <paramref name="xmlTemplate"></paramref>, with the value of the given <paramref name="oCell">cell</paramref>
	''' </summary>
	''' <param name="xmlTemplate"></param>
	''' <param name="xmlDocTemplate"></param>
	''' <param name="oValue"></param>
	''' <remarks></remarks>
	Sub AddObjectType(ByVal xmlTemplate As XmlElement, ByVal xmlDocTemplate As XmlDocument, ByVal oValue As String)
		'--- If it's empty, it won't create any element
		If oValue <> "" Then
			Dim xmlObjectType = xmlDocTemplate.CreateElement("ObjectType")
			xmlObjectType.InnerText = oValue
			xmlTemplate.AppendChild(xmlObjectType)
		End If
	End Sub


	''' <summary>
	''' Adds a new xmlnode with nodename Value and the argument strValue as its innerText
	''' </summary>
	''' <param name="xmlEl"></param>
	''' <param name="xmlDoc"></param>
	''' <param name="strValue"></param>
	''' <param name="bClearFirst">Determines if you want to clear current childnodes before adding</param>
	''' <remarks></remarks>
	Public Sub AddValueTagToNode(ByRef xmlEl As XmlElement, ByRef xmlDoc As XmlDocument, ByVal strValue As String, Optional ByVal bClearFirst As Boolean = True)
		If bClearFirst Then xmlEl.InnerXml = String.Empty

		If Not String.IsNullOrEmpty(strValue) Then
			Dim tmp = xmlDoc.CreateElement("Value")
			tmp.InnerText = strValue
			xmlEl.AppendChild(tmp)
		Else
			xmlEl.ParentNode.RemoveChild(xmlEl)	'no empty value tags
		End If
	End Sub

	''' <summary>
	''' Returns xmlnode with nodename Attribute and the attribute name <paramref name="strName"></paramref>
	''' </summary>
	''' <param name="xmlDoc"></param>
	''' <param name="strName"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function GetAttributeNode(ByRef xmlDoc As XmlDocument, ByVal strName As String) As XmlElement
		Dim tmp = xmlDoc.CreateElement("Attribute")
		tmp.SetAttribute("name", strName)
		Return tmp
	End Function

	''' <summary>
	''' Creates the template Attribute value tags using the values in the given range
	''' </summary>
	''' <param name="xmlTemplate"></param>
	''' <param name="xmlDocTemplate"></param>
	''' <param name="itemsAttribute"></param>
	''' <remarks></remarks>
	Sub FillTemplateFromAttributes(ByVal xmlTemplate As XmlElement, ByVal xmlDocTemplate As XmlDocument, ByVal itemsAttribute As SSA.NameValue)
		Dim xmlAttrNode = GetAttributeNode(xmlDocTemplate, itemsAttribute.name)

		AddValueTagToNode(xmlAttrNode, xmlDocTemplate, itemsAttribute.Value(0).ToString, False)

		xmlTemplate.AppendChild(xmlAttrNode)
	End Sub

	''' <summary>
	''' Creates the template Attribute value tags using the values in the given range
	''' </summary>
	''' <param name="xmlTemplate"></param>
	''' <param name="xmlDocTemplate"></param>
	''' <param name="itemsAttribute"></param>
	''' <remarks></remarks>
	Sub AddAttributes(ByVal xmlTemplate As XmlElement, ByVal xmlDocTemplate As XmlDocument, ByVal itemsAttribute() As SSA.NameValue)
		For Each att In itemsAttribute
			Dim xmlAttrNode = GetAttributeNode(xmlDocTemplate, att.name)

			AddValueTagToNode(xmlAttrNode, xmlDocTemplate, att.Value(0).ToString, False)

			xmlTemplate.AppendChild(xmlAttrNode)
		Next


	End Sub


	''' <summary>
	''' Creates a file node in the xmlDocument
	''' </summary>
	''' <param name="parentElement"></param>
	''' <param name="oFile"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function createFileXmlElement(ByVal parentElement As XmlDocument, ByVal oFile As SSA.File) As XmlElement
		Dim itemObject As XmlElement
		itemObject = parentElement.CreateElement("File")

		'--- Adds the DisplayName
		AddDisplayName(itemObject, parentElement, oFile.DisplayName)

		'--- Adds the ObjectType
		AddObjectType(itemObject, parentElement, oFile.ObjectType)

		'--- Fills the Attributes of the Node
		For Each att In oFile.Attribute
			FillTemplateFromAttributes(itemObject, parentElement, att)
		Next

		Return itemObject
	End Function

	''' <summary>
	''' Creates a document node in the xmlDocument, this method does not append the item object to the document, you have to do that afterwards
	''' This is done like this since there might be files nested on it, so then the document node should not be appended to the document unless it's complete with the nested nodes in there
	''' </summary>
	''' <param name="parentElement"></param>
	''' <param name="oDocument"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function createDocumentXmlElement(ByVal parentElement As XmlDocument, ByVal oDocument As SSA.Document) As XmlElement
		Dim itemObject As XmlElement
		itemObject = parentElement.CreateElement("Document")

		'--- Adds the DisplayName
		AddDisplayName(itemObject, parentElement, oDocument.DisplayName)

		'--- Adds the ObjectType
		AddObjectType(itemObject, parentElement, oDocument.ObjectType)

		'--- Fills the Attributes of the Node
		For Each att In oDocument.Attribute
			FillTemplateFromAttributes(itemObject, parentElement, att)
		Next

		Return itemObject
	End Function

End Module
