// IL code injection for process blocker 
// You can use this IL code injection when you dont have the source files.

    A:  ldstr      "c:\\users\\vmadministrator\\desktop"
    B:  stloc.0
    C:  ldloc.0
    D:  ldstr      "\\ProcessBlocker.dll"
    E:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    F:  call       class [mscorlib]System.Reflection.Assembly [mscorlib]System.Reflection.Assembly::LoadFrom(string)
    G:  ldstr      "ProcessBlocker"
    H:  callvirt   instance class [mscorlib]System.Type [mscorlib]System.Reflection.Assembly::GetType(string)
    I:  ldstr      "Block"
    J:  callvirt   instance class [mscorlib]System.Reflection.MethodInfo [mscorlib]System.Type::GetMethod(string)
    K:  ldnull
    L:  ldc.i4.1
    M:  newarr     [mscorlib]System.Object
    N:  dup
    O:  ldc.i4.0
    P:  ldloc.0
    Q:  stelem.ref
    R:  callvirt   instance object [mscorlib]System.Reflection.MethodBase::Invoke(object,
                                                                                        object[])
