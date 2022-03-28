using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows;

// Общие сведения об этой сборке предоставляются следующим набором
// набор атрибутов. Измените значения этих атрибутов, чтобы изменить сведения,
// связанные со сборкой.
[assembly: AssemblyTitle("SophiApp")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("The Sophia Project")]
[assembly: AssemblyProduct("SophiApp")]
[assembly: AssemblyCopyright("Copyright © 2019–2022")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Установка значения False для параметра ComVisible делает типы в этой сборке невидимыми
// для компонентов COM. Если необходимо обратиться к типу в этой сборке через
// из модели COM, установите атрибут ComVisible для этого типа в значение true.
[assembly: ComVisible(false)]

//Чтобы начать создание локализуемых приложений, задайте
//<UICulture>CultureYouAreCodingWith</UICulture> в файле .csproj
//в <PropertyGroup>. Например, при использовании английского (США)
//в своих исходных файлах установите <UICulture> в en-US.  Затем отмените преобразование в комментарий
//атрибута NeutralResourceLanguage ниже.  Обновите "en-US" в
//строка внизу для обеспечения соответствия настройки UICulture в файле проекта.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //где расположены словари ресурсов по конкретным тематикам
                                     //(используется, если ресурс не найден на странице,
                                     // или в словарях ресурсов приложения)
    ResourceDictionaryLocation.SourceAssembly //где расположен словарь универсальных ресурсов
                                              //(используется, если ресурс не найден на странице,
                                              // в приложении или в каких-либо словарях ресурсов для конкретной темы)
)]

// Сведения о версии для сборки включают четыре следующих значения:
//
//      Основной номер версии
//      Дополнительный номер версии
//      Номер сборки
//      Номер редакции
//
// Можно задать все значения или принять номера сборки и редакции по умолчанию
// используя "*", как показано ниже:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("0.0.0.0")]
[assembly: AssemblyFileVersion("0.0.0.0")]
[assembly: NeutralResourcesLanguage("")]