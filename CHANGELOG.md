# Full Change Log

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
* Formaly added Windows 10 22H2 support;
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

* Startup checkings updated;
  * Now SophiApp checks whether Microsoft Defender was disabled via policies.
* Updated the German translation. Huge thanks to [Henry2o1o](https://github.com/Henry2o1o);
* Added the ability to download the latest stable build of SophiApp via PowerShell. Just type the command below, and it will download SophiaApp.zip from GitHub to the Downloads folder, expand the archive, and remove it. Elevated privileges are not needed. The app won't be launched.

  ```powershell
  irm app.sophi.app | iex
  ```

https://imgur.com/a/wCcs0Xi

## 1.0.0.50 — 08.04.2022

* UI render engine updated;
  * It is now possible to choose where to put the new ID in the JSON config without breaking the sequence of the IDs.
* Startup checkings updated;
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
  * [Screeshots](https://github.com/Sophia-Community/SophiApp#screenshots-helen)
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
