# ScreenSoundSwitch

ScreenSoundSwitch is a C# .NET 8.0 application that manages audio playback devices based on which screen a process window is located on. It consists of a WinUI application for Windows desktop users and a Web API for remote management.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Prerequisites and Installation
- **Platform Requirements**: Primary development requires Windows 10+ with .NET 8.0 SDK
- **Cross-Platform Development**: Limited - WebAPI component only on Linux/macOS
- **Install .NET 8.0 SDK**: Download from https://dotnet.microsoft.com/download/dotnet/8.0

### Building the Solution
- **Full Solution Restore**: `dotnet restore` -- **FAILS on Linux** due to Windows-targeting projects
- **Cross-Platform Build**: 
  - `dotnet restore ScreenSoundSwitch.WebAPI/ScreenSoundSwitch.WebAPI.csproj` -- takes 25-30 seconds. NEVER CANCEL.
  - `dotnet restore SoundSwitch.Common/SoundSwitch.Common.csproj` -- takes 5-6 seconds
- **WebAPI Build**: `dotnet build ScreenSoundSwitch.WebAPI/ScreenSoundSwitch.WebAPI.csproj` -- takes 9-10 seconds. NEVER CANCEL.
- **Common Library Build**: `dotnet build SoundSwitch.Common/SoundSwitch.Common.csproj` -- takes 1-2 seconds

### Windows-Specific Builds
- **Enable Windows Targeting**: Use `-p:EnableWindowsTargeting=true` for Windows projects on non-Windows systems
- **Audio Manager Restore**: `dotnet restore SoundSwitch.Audio.Manager/SoundSwitch.Audio.Manager.csproj -p:EnableWindowsTargeting=true` -- takes 10-12 seconds. NEVER CANCEL.
- **WinUI Projects**: Cannot fully build on Linux due to Windows-specific tools (cswinrt.exe)

### Resource File Workaround (Linux/macOS)
- **Issue**: Case-sensitive filesystems cause build failures with icon resources
- **Solution**: Create lowercase symlinks in SoundSwitch.Common directory:
  ```bash
  cd SoundSwitch.Common
  ln -s Resources resources
  cd resources
  cp defaultMicrophone.ico defaultmicrophone.ico
  cp defaultSpeakers.ico defaultspeakers.ico
  ```

### Running the Application
- **WebAPI Development**: 
  - `dotnet run --project ScreenSoundSwitch.WebAPI/ScreenSoundSwitch.WebAPI.csproj --urls "http://localhost:5254"`
  - Default port 5253 configured in launchSettings.json
  - Swagger UI available at `/swagger` endpoint
  - **Database Required**: Configured for SQL Server (connection string in appsettings.json)
- **WinUI Application**: Can only run on Windows - targets .NET 8.0-windows10.0.19041.0

## Testing and Validation

### Manual Testing Scenarios
- **WebAPI Health Check**: 
  1. Start WebAPI: `dotnet run --project ScreenSoundSwitch.WebAPI/ScreenSoundSwitch.WebAPI.csproj --urls "http://localhost:5254"`
  2. Access Swagger UI: `curl http://localhost:5254/swagger/index.html`
  3. Test endpoint: `curl http://localhost:5254/api/test/check-connection` (requires authentication)
- **No Automated Tests**: Repository contains no unit test projects - only a TestController for database connectivity

### Build Configurations
- **Available Configurations**: Debug, Release, Nightly
- **Debug Build**: Default configuration with full debugging symbols
- **Release Build**: `dotnet build --configuration Release` -- optimized, takes 2-3 seconds. NEVER CANCEL.
- **Nightly Build**: Development configuration for continuous integration

### Docker Support
- **WebAPI Containerization**: Dockerfile available in ScreenSoundSwitch.WebAPI/
- **Build Command**: `docker build -t screensoundswitch-webapi -f ScreenSoundSwitch.WebAPI/Dockerfile .`
- **Docker Build Time**: 15-20 minutes. NEVER CANCEL. Set timeout to 30+ minutes.

## Validation Requirements

### Code Quality
- **No Linting Configuration**: No explicit linting tools configured
- **Warnings**: Expect nullable reference warnings in WebAPI (23+ warnings normal)
- **Build Success Criteria**: Zero errors, warnings acceptable

### Functional Testing
- **ALWAYS** test WebAPI startup after making changes to WebAPI components
- **ALWAYS** verify build succeeds for any modified project
- **ALWAYS** test resource loading if modifying SoundSwitch.Common

## Project Structure and Key Locations

### Solution Projects
1. **ScreenSoundSwitch.WinUI** - Main desktop application (.NET 8.0-windows)
   - MVVM architecture with ViewModels and Views
   - Audio device management UI
   - Process window monitoring
   - Screen detection and audio routing
2. **ScreenSoundSwitch.WebAPI** - REST API (.NET 8.0)
   - User authentication with JWT
   - Audio file management
   - Database integration (Entity Framework)
   - Swagger documentation
3. **ScreenSoundSwitch.WinUI.Audio** - Audio interface library (.NET 8.0-windows)
   - Channel audio volume control
   - Audio device management abstractions
4. **SoundSwitch.Audio.Manager** - Core audio switching logic (.NET 8.0-windows)
   - Window monitoring via Win32 API
   - Audio device switching implementation
   - NAudio integration
5. **SoundSwitch.Common** - Shared utilities (.NET Standard 2.1)
   - Common audio device abstractions
   - Resource management (icons)
   - Shared data structures

### Important File Locations
- **Main Solution**: `ScreenSoundSwitch.sln` (469 total lines across all project files)
- **WebAPI Configuration**: `ScreenSoundSwitch.WebAPI/appsettings.json`
- **Database Context**: `ScreenSoundSwitch.WebAPI/ApplicationDbContext.cs`
- **Audio Resources**: `SoundSwitch.Common/Resources/` (defaultMicrophone.ico, defaultSpeakers.ico)
- **WinUI Entry Point**: `ScreenSoundSwitch.WinUI/MainWindow.xaml`

### Key Dependencies
- **NAudio**: Audio manipulation library
- **Entity Framework Core**: Database ORM for WebAPI
- **Microsoft.Windows.CsWinRT**: Windows Runtime interop
- **JWT Bearer**: Authentication for WebAPI
- **Swagger/OpenAPI**: API documentation

## Common Tasks

### Adding New Audio Device Support
1. Modify `SoundSwitch.Common/Framework/Audio/Device/` classes
2. Update `SoundSwitch.Audio.Manager/AudioSwitcher.cs`
3. Test with `dotnet build SoundSwitch.Common/SoundSwitch.Common.csproj`

### Extending WebAPI Functionality
1. Add controllers in `ScreenSoundSwitch.WebAPI/Controllers/`
2. Update models in `ScreenSoundSwitch.WebAPI/Models/`
3. Test with WebAPI startup and Swagger validation

### Modifying WinUI Interface
1. Update XAML in `ScreenSoundSwitch.WinUI/Views/`
2. Implement logic in corresponding ViewModels
3. **Cannot test on Linux** - requires Windows development environment

### Working with Resources
1. Always work in `SoundSwitch.Common/Resources/` directory
2. Remember case sensitivity on non-Windows systems
3. Update `SoundSwitch.Common/Properties/Resources.resx` if adding new resources

## Known Limitations

### Cross-Platform Development
- **Windows Projects**: ScreenSoundSwitch.WinUI, ScreenSoundSwitch.WinUI.Audio, SoundSwitch.Audio.Manager require Windows
- **Linux/macOS Development**: Limited to ScreenSoundSwitch.WebAPI and SoundSwitch.Common
- **Resource File Issues**: Case sensitivity requires manual workarounds on non-Windows

### Build Environment
- **Windows-Specific Tools**: cswinrt.exe only available in Windows environments
- **Database Dependency**: WebAPI requires SQL Server for full functionality
- **No CI/CD**: No GitHub Actions or automated build pipelines configured

### Testing Infrastructure
- **No Unit Tests**: Manual testing required for all changes
- **No Integration Tests**: Database and audio device testing must be manual
- **No UI Tests**: WinUI interface validation requires manual verification

## Troubleshooting

### Build Failures
- **Resource not found**: Implement case-sensitivity workaround for SoundSwitch.Common
- **Windows targeting errors**: Add `-p:EnableWindowsTargeting=true` to build commands
- **Port already in use**: Change WebAPI port with `--urls` parameter

### Runtime Issues
- **Database connection**: Verify SQL Server availability and connection string
- **Authentication failures**: Check JWT configuration in appsettings.json
- **Audio device access**: Requires Windows environment for full functionality

Always build and exercise your changes manually. Run the WebAPI after modifications to ensure functionality remains intact.