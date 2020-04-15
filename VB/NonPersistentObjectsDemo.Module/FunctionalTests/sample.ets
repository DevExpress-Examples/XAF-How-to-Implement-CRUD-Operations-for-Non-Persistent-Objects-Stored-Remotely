;#DropDB NonPersistentObjectsDemoEasyTest

#Application NonPersistentObjectsDemoWin
#Application NonPersistentObjectsDemoWeb

*FillForm
 User Name = Admin
*Action Log In

#IfDef NonPersistentObjectsDemoWin

*Action About...
 
*Action OK

#EndIf

#IfDef NonPersistentObjectsDemoWeb
*Action Navigation(My Details)
#EndIf
