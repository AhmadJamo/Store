# 06 — التنقل والمسارات

> الحالة: Architecture contract
> مصادر الحقيقة: `app.routes.ts`، `NavigationRegistryService`، permissions، وroute coverage المولدة

## 1. القرار

تبقى عناوين URL الحالية والعقود الخلفية مستقرة أثناء إعادة الواجهة. لا تعاد تسمية 196 route لمجرد تحسين التنظيم.
يُنشأ model عرض فوق المسارات الحالية يربط كل route بـApp وSection وArchetype وPermission وBreadcrumb metadata.
المصدر التنفيذي الوحيد لوجهات المستخدم هو navigation registry؛ route config يبقى مصدر التحميل والحراسة.

## 2. مستويات التنقل

```text
App Launcher
  └─ Current App
      ├─ Section / Destination
      │   ├─ List or Workspace
      │   └─ Record / Form / Related record
      └─ Search / Favorites / Recent (permission-filtered)
```

- App Launcher: وحدات فقط.
- Current App Navigation: وجهات المستوى الأعلى للوحدة.
- Breadcrumbs: السياق داخل المسار، لا نسخة أخرى من menu.
- Open Work: استعادة سياقية، لا authority جديد للمسارات.
- Global Search: route/record results آمنة ومصفاة بالصلاحيات.

## 3. خريطة التطبيقات

| App ID مستهدف | Route families الحالية | Default candidate | ملاحظات |
|---|---|---|---|
| sales | `/sales/**`, `/customers/**` ذات صلة | sales hub | legacy immediate sales تبقى وجهة معنونة بوضوح |
| purchases | `/purchases/**`, `/suppliers/**` ذات صلة | purchasing hub | procurement + payables |
| inventory | `/inventory/**`, `/warehouses/**`, `/stock-*` | inventory hub | العمليات حسب permission/branch |
| products | `/products/**`, categories, attributes, recipes | products list | lookup authority مشتركة |
| contacts | `/customers/**`, `/suppliers/**` | contacts hub أو customers | يمنع تكرار الصفحات في launcher |
| accounting | `/accounting/**` | accounting hub | reports تبقى ضمن الوحدة |
| pos | `/pos/**` والمسارات الحالية ذات الصلة | POS register/session | workspace مستقل |
| analytics | `/analytics/**` | analytics | builder/access وجهات داخلية |
| administration | users, roles, branches, access | administration hub | permission-sensitive |
| settings | `/settings/**`, document design, integrations | settings | grouped searchable settings |

التعيين التفصيلي لكل route يستند إلى `ROUTE_FUNCTION_COVERAGE.ar.md` ولا يعاد نسخه يدوياً هنا كي لا ينشأ مصدر حقيقة ثانٍ.

## 4. Navigation metadata المستهدفة

كل destination مرئية تحتاج:

```ts
interface NavigationDestination {
  id: string;
  appId: AppId;
  sectionId: string;
  route: string;
  routeMatch: 'exact' | 'prefix' | 'record';
  labelKey: NavigationTextKey;
  icon: IconName;
  permission?: string;
  archetype: 'list' | 'record' | 'form' | 'kanban' | 'dashboard' | 'workspace' | 'settings';
  mobilePriority?: 'primary' | 'secondary' | 'hidden';
}
```

بدأ تنفيذ هذا العقد في شريحة FRD-E الأولى بتاريخ 2026-11-24: يعرّف `NavigationRegistryService` التطبيقات العشرة صراحةً مع اسم مترجم وأيقونة ومرشحي دخول آمنين وعضوية route/وجهة. `visibleApplications()` يختار أول وجهة مسموحة فقط، و`applicationForRoute()` يزود الـShell بسياق التطبيق دون تغيير URL. تبقى `sectionId` و`archetype` و`mobilePriority` ضمن الشرائح التالية؛ `permission` يستخدم authority الحالية ولا يكرر منطق الخادم.

## 5. Route classification

| Class | مثال | يظهر في menu؟ | Workspace identity |
|---|---|---:|---|
| Hub | `/sales` | نعم | route ثابت |
| List | `/sales/orders` | نعم | route + filter context الآمن |
| Create | `/sales/quotations/new` | لا، action من List | destination + `new` |
| Record | `/sales/orders/:id` | لا | destination + record id |
| Edit | `/products/:id/edit` | لا | destination + record id/edit |
| Related action | access/reversal/receipt | غالباً لا | parent/record context |
| Settings | `/settings/**` | حسب المجموعة | settings section |
| Public exception | login/welcome/portal token | لا في shell | خارج workspace الداخلي |

## 6. Breadcrumb rules

- App > Destination > Record title/number.
- لا يظهر أكثر من 3–4 مستويات؛ التفاصيل التقنية لا تتحول لمسار بصري.
- record label يأتي من البيانات، مع skeleton عند التحميل وfallback محايد.
- كل segment القابل للنقر يمر عبر permission-aware navigation.
- في الهاتف يظهر parent/back + current title بدلاً من سلسلة طويلة.

## 7. Deep links وعودة المستخدم

- كل List/Record/Form مهم يملك URL قابل للفتح مباشرة.
- filters/sort/page في query params فقط إن كانت غير حساسة ومسموحة للاستعادة.
- الأسرار والتوكنات والـdraft payload لا تخزن في workspace/local storage.
- العودة من Record تحفظ list context المعتمد؛ fallback إلى default list.
- dirty guard يعترض App switch وbranch switch وopen-work close والتنقل العادي بنفس العقد.

## 8. Permission behavior

- إخفاء الوجهة غير المسموحة لتحسين التجربة، مع بقاء guard والخادم سلطة الحماية.
- child action تحتاج permission الخاصة بها، لا تكتفي بصلاحية View للأب.
- نتيجة search/recent/favorite تعاد تصفيتها عند كل قراءة وتغيير جلسة/فرع.
- destination بلا permission أو endpoint صالح لا تظهر كقشرة فارغة.
- access denied صفحة مستقلة، ولا تستخدم كـempty state عادي.

## 9. Module switch behavior

1. المستخدم يفتح launcher أو app switcher.
2. تظهر التطبيقات التي تحوي وجهة واحدة مسموحة على الأقل.
3. اختيار App يفتح آخر وجهة آمنة داخله أو default candidate.
4. current app navigation يتغير كوحدة واحدة.
5. العمل المفتوح في الوحدات الأخرى يبقى في قائمة Open Work ولا يبقى كشريط tabs.

لا يؤدي switch إلى تغيير branch أو workspace profile تلقائياً.

## 10. Search, recent, favorites

- Command search يجمع destinations ثم record preview فقط من providers المسموحة.
- النتائج مجمعة حسب التطبيق، وتعلن type/context للوحة المفاتيح وقارئ الشاشة.
- Favorites تحفظ destination IDs الآمنة، لا URLs عشوائية.
- Recent مشتقة من workspace/history ومحدودة العدد، ولا تحفظ query حساسة.
- عند حذف destination أو سحب permission تتجاهل الحالة المخزنة بأمان.

## 11. Mobile navigation

- top bar: launcher/back، current title، primary action، More.
- app navigation داخل drawer مع الأقسام.
- لا horizontal mega-menu ولا breadcrumbs طويلة.
- bottom navigation غير معتمدة حالياً لأنها لا تناسب 10 تطبيقات وصلاحيات متغيرة.
- browser back يحافظ على السلوك المتوقع ولا يحبس المستخدم داخل overlay.

## 12. الاستثناءات

المسارات التالية لا تستخدم ERP shell الداخلي إلا بقرار خاص:

- `platform-login`, `platform-control`, `platform-management`, `platform-audit`, `platform-retention`.
- `welcome`, `pricing`.
- `supplier-portal/:token`.
- `login`, `register`, `onboarding`, `access-denied` بحسب حالة الجلسة.
- print/PDF/document snapshots.

لكل استثناء layout وهوية وحراسة مناسبة، مع بقاء localization/accessibility.

## 13. خطة الترحيل

1. إضافة metadata للـapps والـarchetypes حول registry الحالي دون تغيير routes.
2. إثبات Shell/App switcher على جميع الصلاحيات واللغتين.
3. Products List/Form/Record pilot مع deep-link وdirty/return context.
4. Sales document pilot لإثبات status/actions/related records.
5. توليد report يكتشف route بلا classification وdestination بلا route.
6. rollout حسب الوحدات، ثم إزالة العرض القديم بعد coverage واختبارات قبول.

## 14. اختبارات القبول

- registry ↔ route coverage دون orphan visible destinations.
- permission matrices لتطبيق كامل، وجهة، وإجراء child.
- direct deep links، browser back/forward، refresh restoration.
- dirty guard لكل أنواع الخروج بما فيها branch/app switch.
- AR/RTL وEN/LTR على desktop/tablet/mobile.
- keyboard: launcher، drawer، app menus، search، open work، breadcrumbs.
- state invalidation بعد permission/branch/session changes.
- 404 وaccess denied وremoved destination لا تسبب loop أو كشف معلومات.
