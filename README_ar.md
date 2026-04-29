

---

## عن SophiApp

> **ملاحظة**: `SophiApp` هو تطبيق مجاني ومفتوح المصدر لضبط إعدادات `Windows 10` و `Windows 11`. يوفر واجهة مستخدم عصرية، أكثر من 130 تعديلاً فريداً، ويُظهر كيف يمكن تهيئة Windows دون إلحاق أي ضرر به.

---

## التبرعات

يمكنك دعم المشروع عبر:
- YooMoney
- Ko-fi
- USDT (TRC20): `TQtMjdocUWbKAeg1kLtB4ApjAVHt1v8Rtf`

---

## متطلبات النظام

| الإصدار | الاسم التسويقي | البناء | المعمارية | الإصدارات |
|:---:|:---:|:---:|:---:|:---:|
| Windows 11 Insider Preview 23H2 | تحديث 2023 | 25206+ | | Home/Pro/Enterprise |
| Windows 11 22H2 | تحديث 2022 | 22621+ | | Home/Pro/Enterprise |
| Windows 11 21H2 | — | 22000.739+ | | Home/Pro/Enterprise |
| Windows 10 22H2 | تحديث 2022 | 19045.2006+ | x64 | Home/Pro/Enterprise |
| Windows 10 21H2 | تحديث أكتوبر 2021 | 19044.1706+ | x64 | Home/Pro/Enterprise/LTSC |

> **ملاحظة**: راجع سجل تحديثات [Windows 10](https://support.microsoft.com/en-us/topic/windows-10-update-history-857b8ccb-71e4-49e5-b3f6-7073197d98fb) و [Windows 11](https://support.microsoft.com/topic/windows-11-update-history-a19cd327-b57f-44b9-84e0-26ced7109ba9) و [Windows 11 Insider Preview](https://docs.microsoft.com/en-us/windows-insider/flight-hub/).

---

## التثبيت

### تحميل SophiApp عبر PowerShell / Chocolatey / Scoop

لتحميل أحدث نسخة من SophiApp، شغّل الأمر التالي في PowerShell (بدون صلاحيات المسؤول):

```powershell
irm app.sophi.app -useb | iex
```

عبر [Chocolatey](https://community.chocolatey.org/packages/sophiapp):

```powershell
choco install sophiapp --confirm
```

عبر [Scoop](https://scoop.sh/#/apps?q=sophiapp):

```powershell
scoop bucket add extras
scoop install sophiapp
```

[الإصدارات التجريبية](https://github.com/Sophia-Community/SophiApp/releases)

> **ملاحظة**: التطبيق محمول بالكامل — لا يحفظ أي بيانات في سجل النظام. فقط استخرج مجلد `SophiApp` مع مجلد `Bin` وملف `SophiApp.exe.config`، ثم شغّل `SophiApp.exe`.

---

## تحذير

- يجب أن يكون هناك مستخدم مسؤول واحد فقط مسجّل الدخول عند تشغيل التطبيق.
- 🔥🔥🔥 قد لا يعمل `SophiApp` على نسخ Windows المعدّلة، خاصةً إذا كانت تلك النسخ تُعطّل Windows Defender أو تحذف مكونات النظام الأساسية.

---

## المميزات الرئيسية

- أكثر من **130 تعديلاً**.
- ضبط Windows عبر الطرق الرسمية الموثّقة.
- واجهة مستخدم مُصيَّرة ديناميكياً — لا شيء مُبرمَج بشكل ثابت. 👻
- يعرض الحالة الحالية لكل ميزة على نظامك.
- يستخدم نمط [MVVM](https://en.wikipedia.org/wiki/Model-view-viewmodel).
- دعم تعدد الخيوط (Multithreading).
- تم فحصه بواسطة [محلل ثابت](https://pvs-studio.com/pvs-studio)، بترخيص مُقدَّم من PVS-Studio.
- جميع الإصدارات تُبنى عبر السحابة بـ [GitHub Actions](https://github.com/Sophia-Community/SophiApp/actions).
- دعم الدقة العالية (High DPI).
- محرك بحث مدمج — يمكن البحث عن الوظائف بعناوينها وأوصافها.
- دعم الثيم الداكن والفاتح مع التبديل الفوري.
- ضبط الخصوصية والإرسال التشخيصي.
- ضبط واجهة المستخدم والتخصيص.
- تثبيت أحدث حزم `Microsoft Visual C++ Redistributable 2015–2022 x86/x64`.
- تثبيت أحدث `.NET Desktop Runtime 7 x86/x64`.
- إلغاء تثبيت OneDrive بشكل صحيح.
- إلغاء تثبيت تطبيقات UWP مع عرض قائمة ديناميكية بأسماء الحزم المترجمة والأيقونات الأصلية.
- تحميل وتثبيت [HEVC Video Extensions](https://www.microsoft.com/p/hevc-video-extensions-from-device-manufacturer/9n4wgh0z6vhq) لفتح صيغ `.heic` و `.heif`.
- إنشاء مهمة مجدولة لتنظيف الملفات غير المستخدمة وتحديثات Windows مع إشعار تفاعلي.
- ضبط Windows Security.
- إمكانية نسخ أوصاف وعناوين الوظائف.
- العديد من التعديلات الفريدة.

---

## مقاطع الفيديو

[![YT](https://img.youtube.com/vi/J0cvbVG9TGw/2.jpg)](https://www.youtube.com/watch?v=J0cvbVG9TGw&t=387s) [![YT](https://img.youtube.com/vi/CyA-oAkybFo/2.jpg)](https://www.youtube.com/watch?v=CyA-oAkybFo)

---

## لقطات الشاشة

![صورة](https://github.com/Sophia-Community/SophiApp/raw/master/img/0.gif)
![صورة](https://github.com/Sophia-Community/SophiApp/raw/master/img/1.png)
![صورة](https://github.com/Sophia-Community/SophiApp/raw/master/img/2.png)
![صورة](https://github.com/Sophia-Community/SophiApp/raw/master/img/3.png)

---

## أسماء حزم UWP المترجمة

![صورة](https://github.com/Sophia-Community/SophiApp/raw/master/img/4.png)
![صورة](https://github.com/Sophia-Community/SophiApp/raw/master/img/5.png)

---

## ميزة البحث

![صورة](https://github.com/Sophia-Community/SophiApp/raw/master/img/search.gif)

---

## التبديل الفوري للثيم

![صورة](https://github.com/Sophia-Community/SophiApp/raw/master/img/theme.gif)

---

### إشعارات تفاعلية لمهمة `تنظيف Windows`

![صورة](https://github.com/Sophia-Community/SophiApp/raw/master/img/Toasts.png)

---

## ملاحق

- بعض الوظائف تتطلب الاتصال بالإنترنت. إذا لم يتوفر الاتصال، ستُخفى من الواجهة حتى يعود.
- يمكنك إظهار الوظائف المخفية بتفعيل `الإعدادات المتقدمة` في الإعدادات — ستُميَّز بأيقونة ترس.
- عند إغلاق `SophiApp`، يُنشئ ملف سجل يمكن إرفاقه بتقرير مشكلة أو إرساله إلى مجموعة [Telegram](https://t.me/sophia_chat). لا يحتوي السجل على أي معلومات شخصية.
- قائمة النطاقات التي يتواصل معها التطبيق:
  - https://raw.githubusercontent.com
  - https://github.com
  - https://download.visualstudio.microsoft.com
  - https://builds.dotnet.microsoft.com
  - https://www.google.com
  - https://g.live.com
  - https://oneclient.sfx.ms

---

## الترجمة

لا تتردد في ترجمة الواجهة إلى لغتك بالاعتماد على أحد ملفات [UIData_xx.json](https://github.com/Sophia-Community/SophiApp/tree/master/src/SophiApp/Localizations) وإنشاء ملف [.xaml](https://github.com/Sophia-Community/SophiApp/tree/master/src/SophiApp/Localizations) جديد.

---

## وسائل التواصل

- [![Discord](https://discordapp.com/api/guilds/1006179075263561779/widget.png?style=shield)](https://discord.gg/sSryhaEv79)
- [مجموعة نقاش Telegram](https://t.me/sophia_chat)
- [قناة Telegram](https://t.me/sophianews)
- [Ru-Board](https://forum.ru-board.com/topic.cgi?forum=5&topic=50903)
- [RuTracker.org](https://rutracker.org/forum/viewtopic.php?t=6218047)
- [Comss.ru](https://www.comss.ru/page.php?id=9679)
- [MajorGeeks.Com](https://www.majorgeeks.com/files/details/sophiapp.html)
- [Softpedia](https://www.softpedia.com/get/Tweak/System-Tweak/SophiApp.shtml)
- [Deskmodder.de](https://www.deskmodder.de/blog/2022/04/08/sophiapp-1-0-0-50-als-finale-version-jetzt-auch-in-deutsch)
- [Reddit](https://www.reddit.com/r/Windows11/comments/tzx74s/sophiapp_the_next_chapter_of_the_sophia_script/)
- [My Digital Life](https://forums.mydigitallife.net/threads/win32-sophiapp-for-windows-10-windows-11-1-0-0-50-x64-2022.85225/)
- [DTF](https://dtf.ru/flood/1325292-sophiapp-ili-kak-my-delali-opensors-programmu-dlya-nastroyki-windows-10-11)
- [Habr](https://habr.com/post/683452/)
