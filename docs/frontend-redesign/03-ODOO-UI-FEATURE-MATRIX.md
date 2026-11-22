# 03 — مصفوفة خصائص Odoo UI المستهدفة

> الحالة: مكتملة لبوابة البحث
> المرجع: `02-ODOO-RESEARCH.md`
> القاعدة: لا تصبح الخاصية Implemented أو Verified بلا عقد حقيقي ودليل اختبار

## حالات القرار

- Apply: النمط مناسب ويمكن تصميم عقد MiniStore له.
- Adapt: الفكرة مناسبة لكن التنفيذ يتقيد بعقود المشروع.
- Conditional: لا يظهر إلا عند وجود capability صريحة.
- Exclude: غير مناسب أو غير مدعوم حالياً.

## المصفوفة

| # | Pattern | مرجع Odoo | الواقع الحالي | القرار | التنفيذ المستهدف | معيار التحقق |
|---:|---|---|---|---|---|---|
| 1 | App Launcher | Official visual | pilot modules grid | Apply | permission-aware module tiles | لا تظهر page-level destinations كبلاطات |
| 2 | Global top bar | Web client pattern | branch/search/session موجود | Adapt | app switch + context + user | RTL/LTR وkeyboard |
| 3 | Module navigation | App menus | registry groups | Adapt | current-app destinations | كل Route مسموح قابل للوصول |
| 4 | Breadcrumbs | Control context | route-derived | Apply | module/list/record trail | deep link + back صحيح |
| 5 | Command search | Control/search | موجود | Apply | pages + recent records لاحقاً | permission filtered |
| 6 | List control panel | List/Search docs | غير موحد | Apply | New/Search/Filter/Views/More | Feature capability contract |
| 7 | Text search | Search docs | موجود محلياً | Apply | typed server query | debounce + URL state |
| 8 | Filters | Search docs | 3 shared فقط | Adapt | allowlisted field/operators | server parity |
| 9 | Multi Group By | Search docs | غير عام | Conditional | backend grouping only | لا grouping على page جزئية |
| 10 | Favorites | Search docs | limited local prefs | Adapt | tenant/user query state | لا sensitive values |
| 11 | Shared favorites | Search docs | غير موجود | Exclude | future backend feature | ownership + permission أولاً |
| 12 | List sorting | List docs | متفاوت | Apply | server sort allowlist | stable order + paging |
| 13 | Optional columns | List docs | غير موحد | Adapt | allowlisted columns | persistence scoped |
| 14 | Row selection | List header actions | بعض الجداول | Conditional | bulk toolbar | API bulk action مثبت |
| 15 | Inline edit | Editable list | محدود | Conditional | safe fields only | validation/concurrency |
| 16 | Export/Import | Actions | feature-specific | Conditional | existing endpoints only | permission + bounded file |
| 17 | Kanban cards | Kanban docs | لا عقد مشترك | Adapt | card view over same data | mobile + same totals |
| 18 | Kanban columns | Kanban docs | workflow progress فقط | Conditional | workflow states | command endpoint لكل move |
| 19 | Drag/drop | Kanban docs | غير موجود | Exclude default | explicit opt-in | rowversion + rollback |
| 20 | Quick create | Kanban docs | forms كاملة | Exclude default | master data only if safe | exact minimal validation |
| 21 | Form header | Form docs | headers متفاوتة | Apply | RecordHeader | number/title/actions |
| 22 | Status bar | Form header | 4 progress consumers | Apply | backend state ribbon | لا transition inferred |
| 23 | Form sheet | Form structural docs | local cards | Apply | consistent content sheet | RTL + responsive |
| 24 | Notebook tabs | Form structural docs | local tabs | Adapt | business sections only | keyboard + URL optional |
| 25 | Smart/stat buttons | Form field classes | links متناثرة | Adapt | related counts/links | server-authored count |
| 26 | Editable lines | x2many form concept | forms ديناميكية | Apply | LineItemsGrid | errors per row + keyboard |
| 27 | Chatter | Chatter docs | لا conversation API عام | Exclude | لا UI وهمي | — |
| 28 | Activity panel | Activities docs | audit/evidence موجود | Adapt | read-only timeline | source/time/actor/permission |
| 29 | Attachments | Chatter/form | feature-specific | Conditional | owning APIs only | size/type/auth |
| 30 | Primary + overflow | Release notes mobile | actions مزدحمة | Apply | one primary, rest menu | phone visual test |
| 31 | Dialogs | Form/action flows | confirm متنوع | Apply | shared confirm/wizard shell | focus trap + Escape |
| 32 | Empty/loading/error | View states | coverage قوية | Apply | archetype states | accessible region |
| 33 | Graph | View records | analytics خاص | Conditional | analytics datasets | server aggregation |
| 34 | Pivot | View records | غير عام | Conditional | reports only | dimensional contract |
| 35 | Calendar | View records | لا عقد عام | Conditional | dated entities only | timezone/server query |
| 36 | Activity view | Activities docs | غير موجود | Future | بعد activity model | API/entity/permissions |
| 37 | POS dashboard | POS docs | POS workspace قائم | Adapt | terminals/sessions cards | real session state |
| 38 | POS register | POS docs | existing sales component | Apply/Adapt | touch-first dedicated layout | barcode/touch/keyboard |
| 39 | POS offline | POS docs | غير مثبت | Exclude الآن | future sync design | idempotent sync required |
| 40 | Company/branch selector | multi-company pattern | branch context قائم | Apply | visible persistent context | server revalidation |
| 41 | Permission-driven actions | view groups/context | session guidance | Apply | hide/disable by UX | endpoint enforcement test |
| 42 | Archive vs delete | actions | feature-specific | Adapt | domain semantics | لا generic delete |
| 43 | Record navigation | form/list | Router/workspace | Apply | preserve query/context | refresh/deep link |
| 44 | Mobile card fallback | Studio views | responsive متفاوت | Adapt | archetype-specific | 390px acceptance |
| 45 | Dynamic view engine | XML architecture | typed Angular | Exclude | explicit components | compile-time types |

## أولويات التنفيذ

### P0 — قبل أي Module rollout

- App/Module/Page information architecture.
- Design tokens.
- ListControlBar + EnterpriseList.
- RecordHeader + StatusRibbon + FormSheet.
- Dialog/focus/error states.
- RTL/LTR/mobile contracts.

### P1 — أول Pilot

- Products List/Form/Card.
- Server search/sort/paging parity.
- optional columns وfavorites ضمن scope آمن.
- related stock evidence كـSmartButton خادمي.

### P2 — Document Pilot

- Sales Order header/status/lines/totals/evidence.
- action overflow وصلاحيات مستقلة.
- activity evidence read-only.

### P3 — Conditional views

- Kanban workflow.
- Graph/Pivot/Calendar.
- Bulk operations وinline edit.
- لا تبدأ قبل capability evidence.

## بوابة القبول لكل Pattern

1. مصدر مرجعي موثق.
2. مستهلكان حقيقيان قبل تعميم abstraction، باستثناء shell primitives.
3. API/permission/query capabilities موثقة.
4. loading/empty/error/unauthorized/validation/concurrency states.
5. RTL/LTR/desktop/mobile/keyboard/accessibility.
6. Unit/integration tests.
7. إزالة البديل القديم فقط بعد route acceptance.
