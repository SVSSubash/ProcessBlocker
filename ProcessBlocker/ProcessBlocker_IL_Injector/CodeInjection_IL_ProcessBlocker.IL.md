// IL code injection for process blocker 
// You can use this IL code injection when you dont have the source files.

    A:  ldstr      "C:\\Users\\vmadministrator\\Desktop\\ProcessBlocker.dll"
    B:  call       class [mscorlib]System.Reflection.Assembly [mscorlib]System.Reflection.Assembly::LoadFrom(string)
    C:  ldstr      "ProcessBlocker"
    D:  callvirt   instance class [mscorlib]System.Type [mscorlib]System.Reflection.Assembly::GetType(string)
    E:  ldstr      "Block"
    F:  callvirt   instance class [mscorlib]System.Reflection.MethodInfo [mscorlib]System.Type::GetMethod(string)
    G:  ldnull
    H:  ldnull
    I:  callvirt   instance object [mscorlib]System.Reflection.MethodBase::Invoke(object,
                                                                                        object[])