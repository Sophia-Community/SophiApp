# Title

1. В Настройках добавить переключатель для включения расширенных (читай: специальных) функций. Например, "Расширенные функции". В разделе "Конфиденциальность" нет расширенных функций

2. Функции, относящиеся к расширенным, будут (при включении последних) иметь приписку в рамке, что они относятся к особой группе "Расширенные"

3. В разделе "Конфиденциальность" для следующих функциий сделать 2 радио-кнопки с приведенными ниже элементами (выпадающий список удобен лишь, когда много элементов):

* "Установить уровень сбора диагностических сведений ОС". Элементы: "Минимальный" и "По умолчанию"
* "Изменить частоту формирования отзывов". Элементы: "Никогда" и "Автоматически"

Пример радио-кнопок:

* <https://raw.githubusercontent.com/felixse/FluentTerminal/master/Screenshots/settings.png>
* <https://github.com/felixse/FluentTerminal>

4. Функцию: "Отключить поиск через Bing в меню "Пуск"" отображать только, если регион США

```powershell
(Get-ItemPropertyValue -Path "HKCU:\Control Panel\International\Geo" -Name Nation) -eq 244
```

C# (NetFramework 4.8):

* <https://docs.microsoft.com/en-us/dotnet/api/system.globalization.regioninfo.geoid?view=netframework-4.8>
* <https://docs.microsoft.com/ru-ru/windows/win32/intl/table-of-geographical-locations> (244)

5. Выводить список задач из Планировщика задач списком как у меня в скрипте

<https://github.com/farag2/Windows-10-Sophia-Script/blob/4a18d0c07e810bfb9f73b8b5f694b5a9c423d6ee/Sophia/Sophia.psm1#L360>

Использовать галочки для выделение элементов списка

6. Около кнопки "Выделить все" написать "Выделить все"

7. Сделать так, чтобы плитки функций равномерно заполняли полотно. Чтобы не было как в Tetris

8. Кнопку "Настройки" поднять чуть вверх, чтобы был отступ снизу

9. В заголовке окна указывать версию программы

10. Справа от положения тумблера указывать словами его состояние: "Вкл." или "Откл."
Пример: <https://raw.githubusercontent.com/felixse/FluentTerminal/master/Screenshots/settings.png>

11. Сделать переход между разделами как у <https://github.com/felixse/FluentTerminal>
Видео-пример (с привязкой ко времени): <https://youtu.be/7uepqoXRohQ?t=295>

12. Добавить возможность копировать описание функции, нажав в контекстном меню рамки "Копировать"

13. Функция для экспорта ico из файла. Понадобится в будущем
<https://gist.github.com/darkfall/1656050#gistcomment-1332369>

```powershell
#
Add-Type -AssemblyName System.Drawing
[System.Drawing.Icon]::ExtractAssociatedIcon("C:\WINDOWS\system32\control.exe").ToBitmap().Save("C:\Desktop\1\1.ico")

#
$Path = "$env:SystemRoot\system32\control.exe"
$FileName = "C:\Desktop\1\1.ico"
$Format = [System.Drawing.Imaging.ImageFormat]::Icon
Add-Type -AssemblyName System.Drawing
$Icon = [System.Drawing.Icon]::ExtractAssociatedIcon($Path) | Add-Member -MemberType NoteProperty -Name FullName -Value $Path -PassThru
$Icon.ToBitMap().Save($FileName,$Format)
```

14. Не использовать ли тумблер переключения как в 10?..

==============================================================================================================

## Эффект размытия фона окна

<https://github.com/asm512/blurry-background-WPF/blob/master/src/BlurryWPFBackground/MainWindow.xaml.cs>

## Кастомный scrollviewer

<http://codesdirectory.blogspot.com/2013/01/wpf-scrollviewer-control-style.html>

## LoadingRing

<https://github.com/zeluisping/LoadingIndicators.WPF/blob/master/src/LoadingIndicators.WPF/Styles/LoadingRing.xaml>

## Работа с JSON

<http://www.jsonutils.com/>

<https://jsonformatter.org/json-pretty-print>

## Регулярные выражения

<https://regex101.com/>

<http://alexweinberger.com/main/pinning-network-program-taskbar-programmatically-windows-10/>

## Получить номер билда

```powershell
[System.Environment]::OSVersion.Version.Build
Get-ItemPropertyValue -Path "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion" -Name CurrentBuild
```

Как будет [выглядеть](https://app.zeplin.io/project/5ee37e184f5880b7453b9ea8/screen/5ee3a8c1560275b83f11a13a) ошибка при отсутствии папки "Scripts" рядом с исполняемым файлом. Вместо "Loading" можно написать "Отсутствуют необходимые файлы для запуска", а круг изменить на прямоугольник

## Получить человеческие имена UWP-пакетов

<https://pastebin.com/raw/nDN7M1Hk>

<https://stackoverflow.com/questions/23331385/how-to-obtain-the-display-name-of-installed-metro-apps/23376722#23376722>

<https://stackoverflow.com/questions/23331385/how-to-obtain-the-display-name-of-installed-metro-apps/37231613#37231613>

<https://stackoverflow.com/questions/51480747/get-the-plain-end-user-readable-name-of-uwp-apps-installed-on-a-system/51484211#51484211>
