# 02 — بحث Odoo UI/UX المرجعي

> الحالة: مكتمل لبوابة البحث
> تاريخ الاطلاع: 2026-11-22
> الإصدار المرجعي الأساسي: Odoo 19.0
> الهدف: استخراج أنماط عمل قابلة للتكييف، لا نسخ الكود أو العلامة أو الأصول

## 1. منهج البحث وحدوده

بدأ البحث بروابط Odoo 20.0 الواردة في Master Prompt، لكن أربع صفحات أساسية أعادت Timeout أثناء التحقق. لذلك لا
ينسب هذا التقرير أي سلوك إلى Odoo 20 لم يُتحقق منه. استخدمت وثائق Odoo 19.0 الرسمية وصفحة الإصدار الرسمية، لأنها
متاحة وقابلة للإسناد، مع مصدر بصري رسمي لشاشة App Launcher.

لم تُستخدم متاجر Themes أو المقالات التسويقية كمصدر حاسم. لم يتم الوصول الكامل إلى Web Enterprise source بسبب
قيود robots/licensing، لذلك لا يدعي التقرير استخراج palette دقيقة أو قياسات pixel رسمية. اللغة البصرية النهائية
لـMiniStore ستبقى أصلية وتخضع لقياس بصري داخل التطبيق.

## 2. المصادر الرسمية

| المصدر | الرابط | ما تم التحقق منه |
|---|---|---|
| Odoo 19 View Architectures | https://www.odoo.com/documentation/19.0/developer/reference/user_interface/view_architectures.html | Form/List/Kanban، structural components، optional columns، smart/stat buttons، صلاحيات العرض |
| View Records | https://www.odoo.com/documentation/19.0/developer/reference/user_interface/view_records.html | أنواع Form/List/Search/Kanban/Graph/Pivot/Calendar وغيرها |
| Search, Filter, Group | https://www.odoo.com/documentation/19.0/applications/essentials/search.html | Filters، multi-level Group By، Favorites، saved default searches |
| Studio Views | https://www.odoo.com/documentation/19.0/applications/studio/views.html | سلوك List/Kanban/Form وKanban الافتراضي على الهاتف |
| Activities | https://www.odoo.com/documentation/19.0/applications/essentials/activities.html | أنشطة مرتبطة بالسجل من Chatter/List/Kanban/Activity view |
| Chatter | https://www.odoo.com/documentation/19.0/applications/productivity/discuss/chatter.html | timeline، notes، attachments، followers، traceability |
| POS Workflow | https://www.odoo.com/documentation/19.0/applications/sales/point_of_sale/use.html | فتح register، البيع، customer، refund، cash، close، access levels، offline state |
| Odoo 19 Release Notes | https://www.odoo.com/odoo-19-release-notes | أزرار Form على الهاتف، action overflow، تحسينات activities وdocument sections |
| App Launcher visual | https://www.odoo.com/documents/content/HLo6zjeWQnCVAvdPf2uZ6go4af3f?download=0 | شبكة تطبيقات بأيقونات وأسماء واضحة |

## 3. Application Shell وNavigation

### ما يثبته المرجع

- التطبيقات هي نقاط دخول عالية المستوى، وليست قائمة بكل Route.
- داخل التطبيق تنتقل التجربة إلى Control Panel مرتبط بالسياق الحالي: عنوان/مسار، بحث، filters، group by، favorites،
  view switching وإجراءات.
- الـView ليست صفحة منفصلة بالضرورة؛ نفس مجموعة السجلات يمكن عرضها List أو Kanban أو Graph/Pivot عندما يدعمها
  النموذج والبيانات.
- صلاحيات المستخدم والسياق يؤثران في قدرات الـView، لكن عرض العنصر لا يلغي enforcement.

### التكييف مع MiniStore

- App Launcher يعرض Modules فقط؛ Command Search يعثر على الصفحات.
- Module rail أو drawer يفتح الوجهات داخل الوحدة الحالية.
- Router وNavigationRegistry وWorkspaceService تبقى السلطة.
- branch/company context يبقى في top bar، لأنه سياق تشغيلي حقيقي في MiniStore.
- لا نقلد Odoo menu XML أو Owl runtime؛ نستخدم Angular routes وtyped feature capabilities.

## 4. List View

المرجع الرسمي يعرّف List كعرض وتحرير سجلات متعددة. الأعمدة يمكن أن تكون optional show/hide، والـgrouped lists
تجمع القيم الرقمية وتعرض pager للمجموعات الكبيرة. Header buttons قد تطبق على selection.

### سلوك مستهدف

1. Toolbar ثابت البنية: New، search، filter chips، Group By، Favorites، view switch، overflow.
2. قائمة كثيفة لكن قابلة للقراءة، مع selection عندما توجد bulk actions حقيقية.
3. sort/paging/filter عبر Backend.
4. optional columns فقط إذا كانت الأعمدة تأتي من metadata allowlist.
5. حالات Loading/Empty/Error/Unauthorized داخل نفس region.
6. إجراءات الصف ضمن overflow عندما يزيد العدد.

### ما لا يُعمم

- inline edit لا يظهر إذا كان endpoint الحالي لا يدعم update آمن للصف.
- grouping لا يتم محلياً على page جزئية.
- export/import لا يظهر لمجرد وجود زر في Odoo.
- bulk actions لا تنشأ دون API وصلاحية وidempotency واضحين.

## 5. Search وFilters وFavorites

وثائق Odoo 19 تثبت أن البحث والتصفية والتجميع تعمل معاً، وأن أكثر من Group يمكن تركيبه، وأن Favorite يحفظ البحث
وقد يصبح default. المشاركة بين المستخدمين خيار مستقل.

### عقد MiniStore المقترح

- Search state يمثل Query DTO حقيقي، وليس domain string عام.
- FilterBuilder يعرض فقط fields/operators التي يعلنها Feature.
- Group By يحتاج Backend aggregation/group support أو يظل غير متاح.
- Favorites تحفظ presentation/query state فقط ضمن tenant/user scope.
- Shared favorites مستبعدة حتى يوجد Backend ownership وpermission model.
- URL يحتفظ بالحالة القابلة للمشاركة؛ drafts الحساسة لا تدخل URL أو storage.

## 6. Form View

Odoo Form يعرض سجلاً واحداً، ويفصل بين mode القراءة/التعديل. المكونات البنيوية الرسمية تشمل header، sheet، group،
notebook، title، buttons container، footer وchatter. الـstat button يستخدم للوصول إلى معلومات مرتبطة.

### عقد MiniStore المقترح

- RecordHeader: الاسم/الرقم، الحالة، primary action، overflow.
- StatusRibbon: يعرض الحالة القادمة من Backend ولا يحسب transition.
- SmartButtons: روابط وعدّادات خادمية فقط.
- FormSheet: حقول أساسية مرتبة حسب business flow.
- NotebookTabs: تستخدم عند وجود مجموعات مستقلة فعلاً، لا لإخفاء نموذج سيئ.
- LineItemsGrid: للوثائق ذات البنود مع validation واضح وkeyboard flow.
- Activity/Evidence panel: يستخدم audit/evidence الموجود؛ لا يدعي Chatter أو إرسال رسائل إذا لم توجد عقوده.
- Save/Cancel semantics مرتبطة بـdirty guard وrowversion.

## 7. Kanban

Odoo Kanban يعرض records كبطاقات، وقد يجمعها في أعمدة workflow. المصدر الرسمي يذكر drag/drop، quick create،
default grouping وحداً عملياً لعشرة أعمدة ظاهرة.

### قرار MiniStore

- يستخدم Kanban للكيانات التي تستفيد من visual grouping فعلاً.
- Products قد يحصل على Card view غير grouped فوق نفس server page، وليس workflow drag/drop.
- Workflow drag/drop ممنوع ما لم يوجد command endpoint يعيد التحقق من transition والصلاحية وrowversion.
- quick create لا يظهر في الوثائق المالية أو المخزنية الحساسة.
- mobile card view يمكن أن يكون البديل الطبيعي للجدول عندما تضيق الشاشة.

## 8. Activities وChatter

Odoo يربط Activity بسجل ويتيح جدولتها من Chatter أو List أو Kanban أو Activity view. الألوان مرتبطة بموعد الاستحقاق
بشكل متسق. Chatter يسجل التغييرات والملاحظات والمرفقات والمتابعين.

### قرار MiniStore

- لا نبني Chatter وهمياً.
- Audit events وdocument evidence والـworkflow history الحالية تُعرض في ActivityPanel بصفتها read-only evidence.
- إنشاء follow-up activity يؤجل حتى يوجد Entity/API/permission حقيقي.
- لا نخلط semantic audit مع user conversation.
- attachments تظهر فقط في Features التي تملكها عقودها الحالية.

## 9. Mobile behavior

مصدر Studio يذكر أن Kanban يستخدم افتراضياً على الهاتف عندما يكون متاحاً. Release notes لـOdoo 19 تذكر أن أول زر
Form يظهر مباشرة وتنتقل الأزرار الأخرى إلى vertical ellipsis.

### قرار MiniStore

- أهم إجراء واحد يبقى ظاهراً، والثانوي داخل overflow.
- الجداول تتحول إلى cards أو horizontal evidence حسب Feature، لا بطريقة واحدة للجميع.
- module rail يتحول إلى drawer/bottom access.
- branch/company context يبقى قابلاً للوصول ولا يختفي.
- touch targets لا تقل عن 44px في POS، ويفضل ذلك في الإدارة على الهاتف.
- لا تعتمد عملية مالية على hover.

## 10. POS

وثائق Odoo 19 تثبت دورة: إعداد POS، فتح register، تنفيذ المبيعات، اختيار العملاء، returns/refunds، cash register،
وإغلاق register. مستويات الوصول تفصل employee وmanager. كما توثق offline indicator ومقايضة cache/initial load.

### قرار MiniStore

- MiniStore يحتفظ بنموذج session/cash/terminal والقيود المحاسبية الحالي.
- POS shell مستقل touch-first، ولا يرث كثافة شاشات الإدارة.
- open/close/cash movement/variance approval تبقى commands خادمية وصلاحيات مستقلة.
- offline mode غير معلن حالياً؛ لا نظهره حتى يوجد sync/idempotency/cache contract.
- barcode search وkeyboard shortcuts تبقى، مع دعم touch.
- لا يخزن browser cache بيانات فروع أو مستخدمين بلا scope صريح.

## 11. اللغة البصرية

### ما يمكن اعتماده من الملاحظة الموثقة

- hierarchy هادئ: navbar/control panel/content.
- كثافة ERP متوسطة إلى عالية.
- borders خفيفة أكثر من shadows الثقيلة.
- primary action واضح، والإجراءات الثانوية أقل بروزاً.
- status colors دلالية ومتسقة.
- app icons تساعد على سرعة التعرف.
- form sheet يميز السجل عن canvas.
- action overflow يقلل الزحمة، خصوصاً على الهاتف.

### ما لا يدعيه التقرير

- لا توجد هنا palette رسمية دقيقة للـBackend.
- لا توجد قياسات pixel مستخرجة من source Enterprise.
- لا ندعي أن plum الحالي هو لون Odoo أو أنه نهائي.
- لا ننسخ Font Awesome assets أو Enterprise icons أو logos.

سيحدد `05-DESIGN-SYSTEM.md` قياسات MiniStore الأصلية بعد visual prototype وRTL/LTR comparison.

## 12. جدول الأنماط المقبولة والمعدلة والمستبعدة

| النمط | القرار | السبب |
|---|---|---|
| App launcher للوحدات | Apply | يقلل الزحمة ويحترم mental model |
| Control panel موحد | Apply | يعالج اختلاف القوائم |
| Search + filters + group + favorites | Adapt | حسب قدرات Backend لكل Feature |
| List optional columns | Adapt | allowlisted metadata فقط |
| Kanban workflow | Conditional | command endpoint مطلوب |
| Form sheet/header/status | Apply | مناسب للـERP documents |
| Smart buttons | Adapt | أرقام وروابط خادمية |
| Chatter | Exclude الآن | لا توجد عقود conversation عامة |
| Activity timeline | Adapt | audit/evidence read-only أولاً |
| Drag/drop state | Exclude افتراضياً | خطر workflow/concurrency |
| Inline edit | Conditional | يحتاج endpoint وvalidation مناسبين |
| Shared favorites | Exclude الآن | يحتاج ownership/permission model |
| Offline POS | Exclude الآن | لا يوجد sync contract مثبت |
| Mobile primary + overflow | Apply | موثق في Odoo 19 ومناسب للمشروع |
| Dynamic XML view engine | Exclude | لا يناسب Angular/typed contracts |

## 13. مقارنة مختصرة مع الواقع الحالي

| المجال | Odoo pattern | MiniStore الحالي | الفجوة |
|---|---|---|---|
| Launcher | apps عالية المستوى | pilot يعرض 10 modules | يحتاج visual acceptance |
| List control | control panel موحد | local filters في صفحات كثيرة | Shared capability-driven toolbar |
| Views | switch حسب model | معظم الصفحات view واحد | لا يضاف إلا بدعم حقيقي |
| Form | header/sheet/notebook/status | تراكيب feature-owned | Record/Form primitives |
| Related records | stat/smart buttons | روابط متناثرة | server counts + consistent buttons |
| Activity | chatter/activity | audit/evidence متعدد | read-only unified panel |
| Mobile | Kanban/action overflow | responsive متفاوت | archetype-specific mobile |
| POS | session workflow مستقل | cohesive sales component | dedicated touch shell + E2E |

## 14. مخرجات البحث الملزمة للتصميم

1. فصل App عن Page في Navigation.
2. Control Bar واحد قابل للتخصيص بقدرات Feature.
3. List/Form/Document هي الأنماط الأولى؛ Kanban conditional.
4. ActivityPanel لا يسمى Chatter ولا يرسل شيئاً بلا Backend.
5. mobile action hierarchy: primary visible، secondary overflow.
6. Server-side query authority غير قابل للتفاوض.
7. Design tokens أصلية، ولا توجد ألوان منسوخة أو مخمنة.
8. كل pattern يمر عبر feature matrix ومعيار قبول قبل التعميم.

## 15. معيار إغلاق البحث

- [x] تحديد الإصدار المرجعي وتاريخ الاطلاع.
- [x] توثيق تعذر Odoo 20 بدلاً من اختلاق نتائج.
- [x] استخدام مصادر Odoo الرسمية المباشرة.
- [x] دراسة Shell/List/Form/Kanban/Search/Activities/Chatter/Mobile/POS.
- [x] فصل Apply/Adapt/Exclude.
- [x] ربط النتائج بواقع MiniStore.
- [x] إنشاء Feature Matrix مستقلة.
