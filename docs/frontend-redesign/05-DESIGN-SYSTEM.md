# 05 — نظام تصميم MiniStore ERP

> الحالة: Design contract v1
> النبرة: عملية، هادئة، كثيفة بالمعلومة، مستوحاة من وضوح Odoo لا من علامته

## 1. الهوية المرئية

يستخدم النظام Plum داكناً كلون shell/brand، وTeal كلون تفاعل وتمييز. الأسطح محايدة ودافئة قليلاً، والحدود أخف من
النص. لا تنسخ ألوان Odoo أو أي assets تابعة له. الاسم والأيقونات والرسوم أصلية لـMiniStore.

## 2. Semantic tokens

الأسماء التالية هي العقد المستهدف؛ تربط لاحقاً بالمتغيرات الحالية تدريجياً ولا تنشئ namespace ثالثاً.

| Role | Light intent | Dark intent | الاستخدام |
|---|---|---|---|
| brand-shell | deep plum | deeper plum | top bar/app identity |
| action-primary | teal 700 | teal 400 | primary action, active control |
| surface-canvas | warm gray 50 | neutral 950 | خلفية مساحة العمل |
| surface-raised | white | neutral 900 | sheet/popover/dialog |
| surface-muted | neutral 100 | neutral 850 | toolbar/subtle grouping |
| text-strong | neutral 950 | neutral 50 | headings/primary data |
| text-default | neutral 800 | neutral 150 | body/data |
| text-muted | neutral 600 | neutral 400 | metadata/help |
| border-default | neutral 250 | neutral 700 | field/table boundaries |
| focus-ring | cyan/teal high contrast | cyan high contrast | keyboard focus |
| success/warning/danger/info | semantic | semantic | status with icon/text |

### قواعد الألوان

- التباين المستهدف WCAG AA للنص والتحكم؛ لا يعتمد status على اللون وحده.
- لون primary للعمل القابل للتنفيذ، وليس لعناوين وأيقونات عشوائية.
- Plum محصور بالـshell والهوية؛ مساحات العمل فاتحة/محايدة لتقليل الإرهاق.
- صفوف الجداول تستخدم hover/selected/focus حالات مختلفة بوضوح.

## 3. Typography

- Font stack يدعم العربية واللاتينية دون تحميل إجباري من شبكة خارجية.
- body 14–16px حسب الكثافة؛ data tables لا تقل عن 13px.
- page title 24–28px، section title 18–20px، label 12–14px بوزن واضح.
- document numbers/codes تستخدم tabular numerals عند توفرها.
- line-height عربي لا يقل عن 1.5 للنصوص، مع اختبار clipping للحركات.
- لا تستخدم uppercase كإشارة وحيدة؛ العربية لا تملك مكافئاً بصرياً.

## 4. Space, size, radius, shadow

- spacing grid: 4, 8, 12, 16, 24, 32, 48.
- control comfortable height: 40px؛ compact: 32–36px؛ touch target: 44px عندما تكون الواجهة touch-first.
- content max width فقط للنماذج/النص؛ الجداول والتحليلات تستفيد من العرض الكامل.
- radius: 6px controls، 8–10px sheets/popovers؛ لا بطاقات pill كبيرة لكل شيء.
- shadow: none للصفحات والجداول؛ subtle للpopover/sticky header؛ stronger للdialog فقط.
- border يفصل البيانات أكثر من shadow.

## 5. Iconography

- اعتماد مجموعة SVG واحدة mirrored-safe ومتناسقة في stroke/filled style.
- الأيقونة لا تستبدل label في الإجراءات غير الشائعة.
- الأزرار icon-only تحتاج accessible name وtooltip، ومساحة نقر مناسبة.
- أسهم الاتجاه والرجوع تراعي RTL؛ الرموز العالمية مثل plus/search لا تنعكس.
- emoji الحالي في registry يعتبر placeholder ويستبدل تدريجياً بمكوّن icon موحد.

## 6. Component contracts

### 6.1 Buttons

| Variant | الاستخدام |
|---|---|
| Primary | الإجراء الأساسي الوحيد في السياق |
| Secondary | إجراء مهم غير أساسي |
| Ghost | toolbar/navigation |
| Danger | فعل مدمر مؤكد |
| Link | انتقال خفيف داخل النص/السجل |

الحالات: default, hover, active, focus-visible, loading, disabled. Disabled لا يحمل تفسيراً وحيداً؛ يرافقه help عند الحاجة.

### 6.2 Inputs

- label دائم؛ placeholder مثال/تلميح لا label.
- required/optional محليان ومفهومان في اللغتين.
- help قبل الخطأ؛ validation مرتبط بـ`aria-describedby`.
- lookup يعرض الاسم، يدعم بحث الاسم والباركود للمنتج، ولا يعرض ID كقيمة أساسية.
- date/number/currency يحترم locale مع قيمة domain محايدة.

### 6.3 Search and filters

- حقل البحث له clear وshortcut عند ملاءمته.
- filter chip يبين القيمة ويزال بلوحة المفاتيح.
- filter popover يملك Apply/Reset واضحين إذا كانت التغييرات staged.
- saved search يبين personal/shared فقط عندما يدعم الخادم ذلك.

### 6.4 Tables

- sticky header عند طول مناسب، alignment حسب نوع البيانات، وأرقام tabular.
- header sortable يعلن الاتجاه؛ لا تستخدم click على نص غير واضح.
- checkbox column ثابت، actions في الطرف المنطقي end.
- loading row وempty/error داخل مساحة النتائج، لا alert منفصل بعيد.
- mobile يعرض record cards/rows ولا يضغط جدولاً عريضاً إلى نص غير مقروء.

### 6.5 Status and workflow

- badge للحالة القصيرة، banner للمعلومة المؤثرة، progress للحالات المتسلسلة الحقيقية.
- status palette ثابتة دلالياً؛ Draft محايد، In progress/info، Waiting/warning، Done/success، Failed/danger.
- لا توحي الواجهة بإمكانية انتقال غير مسموح.

### 6.6 Overlays

- Popover للاختيارات الخفيفة، drawer للسياق/التنقل على الهاتف، dialog لقرار يحتاج تركيزاً.
- focus trap وreturn focus إلزاميان للdialog/drawer.
- Escape يغلق ما يمكن إلغاؤه، ولا يلغي عملية مكتملة.

### 6.7 Feedback

- toast لتأكيد قصير غير حرج؛ inline feedback للأخطاء القابلة للإصلاح في السياق.
- لا تكرر نفس الخطأ في toast وbanner وfield معاً.
- كل خطأ API يحتفظ بـcorrelation evidence عند توفره.
- skeleton مطابق للبنية، spinner للعمليات المحدودة داخل control.

## 7. Page composition

### List

`PageControlBar → Search/Filters/View → BulkBar (conditional) → Results → Pager`

### Record

`RecordHeader → WorkflowStatus → Sheet → Related tabs/evidence → Activity panel (conditional)`

### Form

`FormHeader → Identity group → Domain sections → Lines → Sticky save/discard`

### Dashboard

`Context filters → KPI strip → decision charts → exception/action list`

## 8. Density

- Comfortable هو الافتراضي العام.
- Compact يغير space/control/row heights فقط، لا يخفي حقولاً أو labels.
- POS له touch density مستقلة ولا يرث compact desktop.
- حفظ التفضيل لكل مستخدم ضمن الآلية الحالية، دون اعتباره صلاحية.

## 9. Motion

- 120–180ms للحالات الصغيرة، 180–240ms للdrawer/dialog.
- لا animation على كل row أو page load.
- احترام `prefers-reduced-motion` بإلغاء الحركة غير الضرورية.
- transitions لا تؤخر حفظ/ترحيل/تنقل فعلي.

## 10. Accessibility contract

- semantic landmarks وعنوان H1 واحد منطقي.
- skip link فعال، focus-visible عالي التباين، وعدم وجود keyboard traps.
- ترتيب tab يطابق DOM والمنطق في RTL/LTR.
- icon-only controls مسماة؛ status/validation معلنة.
- live regions محدودة للنتائج المهمة، لا إعلان كل تحديث صغير.
- zoom 200% وreflow دون فقد إجراءات أساسية.
- forced-colors يبقي الحدود والاختيار والتركيز مقروءة.

## 11. Localization contract

- كل label/message/placeholder/tooltip في shared أو feature localization مع `en-US` و`ar-JO`.
- النصوص تسمح بتمدد 30–40% دون قص.
- استخدام logical properties وBootstrap start/end فقط.
- الأرقام/العملة/التواريخ عبر locale formatters؛ codes/document numbers بلا ترجمة.

## 12. Governance

- صفحة `/design-system` تصبح مختبراً لكل component/state/theme/density/direction.
- لا يضاف primitive مشترك قبل ثبوت مستهلكين حقيقيين أو حاجة shell أساسية.
- كل component جديد يحتاج visual states، keyboard contract، localization، واختبارات مركزة.
- يمنع hard-coded presentation text أو hex values داخل feature SCSS إلا استثناء موثق.
- migration لا تعتبر مكتملة بالـscreenshot؛ يجب إثبات العقود والاختبارات والحالات.

## 13. قبول نظام التصميم

- tokens موحدة بلا namespaces متنافسة.
- demo يغطي light/dark، comfortable/compact، Arabic/English، loading/empty/error/disabled/focus.
- List/Form/Record pilot يستخدم المكونات نفسها دون CSS overrides واسعة.
- static accessibility وunit tests وproduction build وvisual baseline كلها ناجحة.
