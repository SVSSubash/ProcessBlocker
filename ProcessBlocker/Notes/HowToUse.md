1. Add the following lines of code whereever you want to start debugging.
2. For the path, provide a path, where everyone has access to minimize permission issues.
3. Copy and paste the ProcessBlocker.dll in the path.
```
           // Debugger entry point setter.
#if DEBUG
            string path = <@"c:\users\admin\desktop">;
            Assembly assembly = Assembly.LoadFrom(path + @"\ProcessBlocker.dll");
            Type type = assembly.GetType("ProcessBlocker");
            type.GetMethod("Block").Invoke(null, new object[] { path });
#endif
```

