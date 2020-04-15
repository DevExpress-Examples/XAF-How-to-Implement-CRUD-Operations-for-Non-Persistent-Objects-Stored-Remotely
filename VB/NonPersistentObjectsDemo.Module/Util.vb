Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

Namespace NonPersistentObjectsDemo.Module

	Public Class GenHelper
		Private Shared srnd As Random
		Private Shared words As List(Of String)
		Private Shared fnames As List(Of String)
		Private Shared lnames As List(Of String)
		Shared Sub New()
			srnd = New Random()
			words = CreateWords(12000)
			fnames = CreateNames(200)
			lnames = CreateNames(500)
		End Sub
		Private Shared Function CreateWords(ByVal number As Integer) As List(Of String)
			Dim items = New HashSet(Of String)()
			Do While number > 0
				If items.Add(CreateWord()) Then
					number -= 1
				End If
			Loop
			Return items.ToList()
		End Function
		Private Shared Function MakeTosh(ByVal rnd As Random, ByVal length As Integer) As String
			Dim chars = New Char(length - 1){}
			For i As Integer = 0 To length - 1
				chars(i) = ChrW(AscW("a"c) + rnd.Next(26))
			Next i
			Return New String(chars)
		End Function
		Private Shared Function CreateWord() As String
			Return MakeTosh(srnd, 1 + srnd.Next(13))
		End Function
		Private Shared Function CreateNames(ByVal number As Integer) As List(Of String)
			Dim items = New HashSet(Of String)()
			Do While number > 0
				If items.Add(ToTitle(CreateWord())) Then
					number -= 1
				End If
			Loop
			Return items.ToList()
		End Function
		Public Shared Function ToTitle(ByVal s As String) As String
			If String.IsNullOrEmpty(s) Then
				Return s
			End If
			Return String.Concat(s.Substring(0, 1).ToUpper(), s.Substring(1))
		End Function

		Private rnd As Random
		Public Sub New()
			rnd = New Random()
		End Sub
		Public Sub New(ByVal seed As Integer)
			rnd = New Random(seed)
		End Sub
		Public Function [Next](ByVal max As Integer) As Integer
			Return rnd.Next(max)
		End Function
		Public Function MakeTosh(ByVal length As Integer) As String
			Return MakeTosh(rnd, length)
		End Function
		Public Function MakeBlah(ByVal length As Integer) As String
			Dim sb = New StringBuilder()
			For i = 0 To length
				If sb.Length > 0 Then
					sb.Append(" ")
				End If
				sb.Append(GetWord())
			Next i
			Return sb.ToString()
		End Function
		Public Function MakeBlahBlahBlah(ByVal length As Integer, ByVal plength As Integer) As String
			Dim sb = New StringBuilder()
			For i = 0 To length
				If sb.Length > 0 Then
					sb.Append(" ")
				End If
				Dim w = ToTitle(MakeBlah(3 + rnd.Next(plength))) & "."
				sb.Append(w)
			Next i
			Return sb.ToString()
		End Function
		Public Function GetFullName() As String
			Return String.Concat(GetFName(), " ", GetLName())
		End Function
		Private Function GetFName() As String
			Return fnames(rnd.Next(fnames.Count))
		End Function
		Private Function GetLName() As String
			Return lnames(rnd.Next(lnames.Count))
		End Function
		Private Function GetWord() As String
			Return words(rnd.Next(words.Count))
		End Function
	End Class
End Namespace
