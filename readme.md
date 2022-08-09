1. Add 

```
using System.Reflection 

```
   in your source file.
   
2. Add the following lines of code whereever you want to start debugging, in the source file.
3. For the path, provide a path in the remote computer, where everyone (every account) has access to minimize permission issues.
4. Copy and paste the ProcessBlocker.dll in the path provided above.

```
           // Debugger entry point setter.
#if DEBUG
            string path = @"c:\users\John\desktop";
            Assembly assembly = Assembly.LoadFrom(path + @"\ProcessBlocker.dll");
            Type type = assembly.GetType("ProcessBlocker");
            type.GetMethod("Block").Invoke(null, new object[] { path });
#endif

```
or add this lines 

```
           // Debugger entry point setter.
#if DEBUG
            string path = @"c:\users\John\desktop";
            Assembly.LoadFrom(path + @"\ProcessBlocker.dll").GetType("ProcessBlocker").GetMethod("Block").Invoke(null, new object[] { path });
#endif

```
