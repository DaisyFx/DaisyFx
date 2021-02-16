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