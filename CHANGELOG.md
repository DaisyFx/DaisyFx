# Changelog

## Unreleased

### Breaking Changes

- Rename conditional sub-chains ([#3](https://github.com/DaisyFx/DaisyFx/pull/3))
    
  The api for conditional sub-chains has been renamed from `.Conditional` to `.If`
  
  Before:
  ```csharp
  root.Conditional(IsDivisibleBy3, then => then
          .Link<ConsoleWriteLine, string>()
      );
  ```

  After:
  ```csharp
  root.If(IsDivisibleBy3, then => then
          .Link<ConsoleWriteLine, string>()
      );
  ```

- Rename ILink.Invoke ([#1](https://github.com/DaisyFx/DaisyFx/pull/1))

  The `Invoke` method in `StatelessLink` and `StatefulLink` has been renamed to `ExecuteAsync`

 - Change namespace of IServiceCollection and endpoint route extensions ([#12](https://github.com/DaisyFx/DaisyFx/pull/12))  

   Follow best practice and thus simplifies usage when registering the library since the intellisense will pick up the methods without extra usings.
   
 - Use interface instead of implementation for 'DaisyServiceCollection' in contracts ([#12](https://github.com/DaisyFx/DaisyFx/pull/12))  