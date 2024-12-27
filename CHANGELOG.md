# Full Change Log

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## 1.0.97 — 26.07.2023

* Fixed #188;
* `SophiApp` was added to [WinGet](https://github.com/microsoft/winget-pkgs/tree/master/manifests/t/TeamSophia/SophiApp)

  ```powershell
  winget install sophiapp --accept-source-agreements --location D:\folder
  winget install --id=TeamSophia.SophiApp --accept-source-agreements --location D:\folder
  ```

  * Merged #189;

> **Note**: If you installed `SophiApp` via `WinGet`, before removing files, you should uninstall `SophiApp` via `WinGet` first, unless you won't be able to re-download package again.

```powershell
winget uninstall sophiapp --force
```

## 1.0.95 — 02.07.2023

* Updated Italian, German translations;
  * Thanks to @bovirus and @sensinsane.
* Fixed URL for AntiZapret (capable for Russian region only).

## 1.0.94 — 31.01.2023

* Updated description for disabling DiagTrack service;
  * Closes #173.
* Updated Italian translation;
  * Thanks to @bovirus.
* Merged #167
  * Thanks to @SunsetTechuila.

All efforts are directed towards the SophiApp 2.0 development. Read more: <https://t.me/SophiaNews/1311>

## 1.0.93 — 08.01.2023

* #167 closed;
  * Thanks to @SunsetTechuila.
* Please pay attention that these domains have to be whitelisted in you firewall to let `SophiApp` works. The domains the app interacts with related to `Microsoft` resources to check for the latest Visual C++ Redistributables available, .NET Desktop Runtimes and so on;
  * https://raw.githubusercontent.com
  * https://github.com
  * https://download.visualstudio.microsoft.com
  * https://builds.dotnet.microsoft.com
  * https://www.google.com
  * https://g.live.com
  * https://oneclient.sfx.ms
* Minor changes.

## 1.0.92 — 17.12.2022

* Added Polish translation
  * Thanks to @alan-null.
* Update Ukrainian translation
  * Thanks to @lowl1f3

## 1.0.91 — 11.12.2022

* Bumped nuget Json.NET to 13.0.2;
* Added Spanish translation
  * Thanks to @Marcosgt3.
* Minor changes.

## 1.0.90 — 06.12.2022

* Fixed `Sophia` registry key wasn't saving;
 * Please remove and re-create SophiApp scheduled tasks to get all working. 😸 
* Switched .NET Desktop Runtime 6 to the 7th version.

## 1.0.88 — 01.12.2022

* Updated the Scheduled tasks notification toasts UI;
  * ![Image](https://github.com/Sophia-Community/SophiApp/raw/master/img/Toasts.png)
  * Remove and re-create SophiApp scheduled tasks to get new toasts UI.
* Json.NET bumped to 13.0.2;
* Minor changes.

## 1.0.87 — 04.11.2022

* Finally, headings are now bold 😸;
  * #150 closed.

![light](https://user-images.githubusercontent.com/10544660/199981391-3401e761-177a-4dc1-af56-93100856f52e.png)
![dark](https://user-images.githubusercontent.com/10544660/199981407-3d7bec70-3476-4d4f-ba78-f441830ee41e.png)

## 1.0.86 — 30.10.2022

* Small code refactoring;
* Code refactoring for CI/CD scripts;
* Fixed old bug in Meet Now function when it didn't save registry key value;
* The function to open `Windows Terminal` as admin in the context menu item as administrator is back;
  * It fully relies on `Windows Terminal` feature by editing its config. No more 3rd part context menu items.
* Improved all scheduled tasks creation;
  * Unified tasks with [Sophia Script](https://github.com/farag2/Sophia-Script-for-Windows): they are created now in `Sophia` folder;
  * When you remove all tasks in the `Task Scheduler`, folder will be removed too;
  * If you run the app, it will show that you haven't any scheduled tasks created due to they are created in a new folder by default. Create them again, and all your old tasks will be removed without traces.

## 1.0.84 — 21.09.2022

* Action config was totally [re-written](https://github.com/Sophia-Community/SophiApp/blob/master/.github/workflows/SophiApp.yml);
  * Closes #128.
* Fixed the HEVC codec function not working on the latest Windows 11 22H2 build;
* Fixed bug in the function for changing the Start layouts on Windows 22H2 build;
* Removed function regarding adding feature to open Terminal as admin in the context menu item due to its' unnecessity. Terminal does it better via internal settings.
* Fixed typos in descriptions;
* Updated the Ukrainian translation;
  * Thanks to @lowl1f3.

## 1.0.82 — 27.08.2022

* Added the Chinese localization 🇨🇳;
  * Closes #119;
  * Thanks to @wcxu21.
* Fixed typos in the Russian localization;
  * Thanks to @dartraiden.

## 1.0.81 — 24.08.2022

* The minimum required Windows 10 version is now `19044.1706`;
* The app now doesn't block UI if a new version is available;
  * You will get a notification toast only.

## 1.0.80 — 19.08.2022

* Огромная статья на Хабре о том, как шла разработка: <https://habr.com/post/683452/>
* Fixed minor bugs in startup checks;
* Added function to enable proxy by [ProstoVPN.AntiZapret](https://antizapret.prostovpn.org). The function is applicable for Russia only;
* Added function to disable `Search highlights`;
* Now you can download `SophiApp` via [choco](https://community.chocolatey.org/packages/sophiapp). Thanks to @Xav83

  ```powershell
  choco install sophiapp --confirm
  ```

* Minor changes;

SophiApp.zip SHA256 hash ([the Compress Files step](https://github.com/Sophia-Community/SophiApp/runs/7919354110?check_suite_focus=true#step:9:16)): `E0AB6EA3C63DD73E03553A268812CC347B5A3BE09244430B7096D53157E4DDF5`

## 1.0.77 — 04.08.2022

* All startup checking were re-written from the scratch;
  * If Microsoft Defender was disabled or replaced another 3rd party AV, the appropriate functions will be disabled and greyed out.
* Fixed `Configure Start layout` function (for Windows 11 22H2 only) not working;
* Fixed `Download and install the HEVC video codec (H.265)` function for installing HEVC codec not working;
* Increased SophiApp.exe icon up to 256x256 pixels;
* Added Czech 🇨🇿 translation;
  * Thanks to @luciusagarthy.
* Added French 🇫🇷 translation;
* Added Telegram link to the Settings page;
* Removed now unnecessary zero in build, having moved to the `MAJOR.MINOR.PATCH` versioning;
* Minor changes;
* The main [page](https://github.com/Sophia-Community/SophiApp/blob/master/README.md) was translated into Italian 🇮🇹 , and русский 🇷🇺. 

## 1.0.0.70 — 04.07.2022

Diff from v1.0.0.62
[1.0.0.62...1.0.0.70](https://github.com/Sophia-Community/SophiApp/compare/1.0.0.62...1.0.0.70)

* Fixed bug when the app didn't launch without the internet connection;
* Fixed bug when child elements left a blank space in the UI;
  * From this time, if some conditions aren't met for a function to render in the UI, it will be greyed out;
  * ![image](https://i.imgur.com/TxWUTbS.png)
  * Reported by `greglou`.
* Fixed bug in function for adding the "Open in Windows Terminal" (Admin) item in the Desktop and folders context menu when you couldn't open Windows Terminal as admin in a path ends in a backslash `\`;
  * Read more [here](https://github.com/microsoft/terminal/issues/4571);
  * To apply the fix turn off this feature and turn again.
* Fixed bug in the `Set the diagnostic data collection to minimum` function;
  * Now it uses `gpedit.msc` path: `HKLM:\Software\Policies\Microsoft\Windows\DataCollection` instead of `HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection`;
  * To apply the fix set the diagnostic data collection to the default value and then to the minimum back, if you're well aware of the consequences.
* Added Turkish translation <img src="https://upload.wikimedia.org/wikipedia/commons/b/b4/Flag_of_Turkey.svg" height="11px"/> ;
  * Thanks to @daswareinfach.
* Add feature to install the latest .NET Desktop Runtime 6 x86/x64 version;
  * The Internet access required.
  * <https://dotnet.microsoft.com/en-us/download/dotnet/6.0>
* Added startup condition if the "Print Spooler" service was removed from OS. If so, the app will block loading.
* Feature to enable the `Windows 10 File Explorer` was removed;
* Added `SearchHighlights` function to hide search highlights for Windows 10;
* Добавлена страница с описанием на русском: <https://github.com/Sophia-Community/SophiApp/blob/master/README_ru-ru.md>
* Updated translations;
* Formally added Windows 10 22H2 support;
* Code refactoring.

## 1.0.0.60 — 18.05.2022

Diff from v1.0.0.56
[1.0.0.56...1.0.0.60](https://github.com/Sophia-Community/SophiApp/compare/1.0.0.56...1.0.0.60)

* Fixed bug when app stuck while applying settings.
  * #48 closed. Thanks to Vladimir for providing us remote access to his PC.
* Formally, added Windows 10 Enterprise Government homebrew version support as an exception.
  * All Defender related functions will be disabled due to this Windows 10 version has Defender deactivated;
  * #58 closed.
* Added an advanced feature to restore the Microsoft Store and all its dependencies via official method.
  * All advanced features can be enabled in the Settings. They will mark with a gear.
  * Internet connection required.
![Снимок экрана (31)](https://user-images.githubusercontent.com/10544660/167956381-6afa5475-4a48-4d34-ab03-8773b7ffec5c.png)
* Fix a bug in the function that helps to install the latest Microsoft Visual C++ Redistributable Packages 2015–2022 x64 when both `Install` and `Uninstall` were not clickable.

## 1.0.0.56 — 27.04.2022

Diff from v1.0.0.53
[1.0.0.53...1.0.0.56](https://github.com/Sophia-Community/SophiApp/compare/1.0.0.53...1.0.0.56)

* Added Italian translation <img src="https://upload.wikimedia.org/wikipedia/commons/0/03/Flag_of_Italy.svg" height="11px"/> 
  * Thanks to @THEBOSSMAGNUS;
* Added board to the app window;
* The Start menu functions were moved to the Personalization;
* Brought to the similar designs functions
  * PC Health Check app;
  * The latest Microsoft Visual C++ Redistributable Packages 2015–2022 x64;
  * OneDrive.
* Fixed a bug when the app didn't detect correctly whether C++ Redistributable packages' version was installed higher than SophiApp offered.

![Снимок экрана (1)](https://user-images.githubusercontent.com/10544660/165491772-c0c4d344-f890-4253-8819-51925f5de39f.png)

## 1.0.0.53 — 11.04.2022

* Startup checks updated;
  * Now SophiApp checks whether Microsoft Defender was disabled via policies.
* Updated the German translation. Huge thanks to [Henry2o1o](https://github.com/Henry2o1o);
* Added the ability to download the latest stable build of SophiApp via PowerShell. Just type the command below, and it will download SophiaApp.zip from GitHub to the Downloads folder, expand the archive, and remove it. Elevated privileges are not needed. The app won't be launched.

  ```powershell
  iwr app.sophia.team -useb | iex
  ```

https://imgur.com/a/wCcs0Xi

## 1.0.0.50 — 08.04.2022

* UI render engine updated;
  * It is now possible to choose where to put the new ID in the JSON config without breaking the sequence of the IDs.
* Startup checks updated;
* Added Windows 11 & Windows 11 Insider Preview support (22509+). Closes [#40](https://github.com/Sophia-Community/SophiApp/issues/40);
* UI/UX updated;
* Updated and expanded localizations;
* Added <img src="https://upload.wikimedia.org/wikipedia/commons/b/ba/Flag_of_Germany.svg" height="11px"/> German and <img src="https://upload.wikimedia.org/wikipedia/commons/4/49/Flag_of_Ukraine.svg" height="11px"/> Ukrainian translations. Closes [#45](https://github.com/Sophia-Community/SophiApp/issues/45);
* Updated functions' descriptions;
* Huge code refactoring;
* Many bugs fixed.
* [Post](https://t.me/SophiaNews/746) on the Telegram channel about the announcement.

## 1.0.0.13 beta 2 — 31.01.2022

* Uploaded the most up-to-date screenshots that reflects the current state of development;
  * [Screenshots](https://github.com/Sophia-Community/SophiApp#screenshots-helen)
* The `Security` category completed;
* Added description to the `UWP applications` category;
  * Remind you that you may copy any header and description by right-clicking header.
* Removed the frame from the app window;
* Added the ability to search by functions headers and descriptions;
  * [GIF](https://github.com/Sophia-Community/SophiApp#searching-feature)
* Fixed button animation on the warning page if SophiApp is running on unsupported Windows;
* Added a new startup checking: you can't run multiple copies of SophiApp at the same time;
* Added description in the `SophiApp.exe` file properties;
* The states of the OneDrive install and uninstall buttons now look more clear;
* The `For All Users` switch on the `UWP Apps` category was replaced by a checkbox;
* The "RadioButtons Group" element was added a lower bound;
  * Closed #23.
* Add the ability to maximize window;
  * Closed #24.
* Window title bar now displays the app version;
* Minor UI changes;
* Minor bug fixed.

## 1.0.0.0 beta 1 — 30.12.2021

* Initial release after many pre-releases.
