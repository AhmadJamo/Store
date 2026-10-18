# ADR — Customer and business-partner identity boundary

> Date: 2026-10-18  
> Status: Accepted as `CUS-005`  
> النطاق: بوابة قرار توثيقية؛ لا كود أو ترحيل قاعدة بيانات

## السياق / Context

يملك MiniStore حاليًا كيانين مستقلين ومستقرين: `Customer` (الزبون) للمبيعات و`Supplier` (المورد)
للمشتريات. تستخدم المستندات التاريخية `CustomerId` (معرّف الزبون) و`SupplierId` (معرّف المورد) بعلاقات
محمية بالشركة. قد تكون الجهة القانونية نفسها زبونًا وموردًا، لكن إنشاء هوية مركزية الآن سيفرض ترحيلًا واسعًا
على المبيعات والمشتريات والمحاسبة قبل وجود حاجة تشغيلية تبرر الخطر.

MiniStore currently has two stable identities: Customer for sales and Supplier for purchasing. Historical documents
reference their tenant-protected IDs. A legal party may perform both roles, but replacing those IDs now would create
a wide sales, purchasing and accounting migration without a proven operational need.

## القرار / Decision

1. يبقى `Customer.Id` و`Supplier.Id` معرفين رسميين مستقلين ومتوافقين.
2. يبدأ `CUS-010` بتوسيع `Customer` الحالي بصورة إضافية: رمز، ملف قانوني واتصال، حالة، أرشفة، نسخة صف،
   استعلامات مقسمة وواجهات DTO (كائنات نقل بيانات)، دون تغيير معرفه.
3. لا ينشأ جدول `BusinessPartner` (شريك أعمال موحد) ولا مفتاح مركزي في `CUS-010`.
4. إذا ظهرت لاحقًا حاجة مثبتة لربط الجهة ذات الدورين، تبدأ بمرجع ربط اختياري لا يغير أي مفتاح تاريخي.
5. التوحيد الحقيقي يبقى لـ`CUS-110` وبوابة مستقلة، باستخدام `expand-and-contract` (التوسعة ثم التحويل
   ثم الإزالة المؤجلة)، والتحقق المزدوج وخريطة معرفات قديمة قبل أي إيقاف.

Customer.Id and Supplier.Id remain separate compatibility identities. CUS-010 expands Customer additively and
does not introduce BusinessPartner. A future dual-role requirement may first add an optional link without rewriting
historical keys. Actual unification remains a separately approved CUS-110 expand-and-contract migration.

## حدود الملكية / Ownership boundaries

- Customer owns customer profile, archive state and future child contacts/addresses.
- Supplier continues to own the supplier profile and purchasing relationships.
- Sales references Customer; Purchasing references Supplier.
- Accounting owns journals and receivable/payable facts. A customer balance is derived, never editable profile state.
- New commercial documents freeze the necessary customer name/address snapshots; master-data edits never rewrite history.

## ثوابت التوافق / Compatibility invariants

- لا حذف أو إعادة ترقيم أو إعادة استخدام لـ`CustomerId` أو`SupplierId`.
- لا تحويل تلقائي لزبون إلى مورد أو العكس.
- لا تفرد عالمي للاسم أو الهاتف أو البريد أو الرقم الضريبي؛ كشف المكرر منفصل ويحتاج مراجعة بشرية.
- الأرشفة تمنع الاختيار الجديد لكنها لا تخفي المستندات السابقة.
- أي كيان تابع جديد يحمل حدود الشركة والعلاقات المركبة نفسها.

## البدائل المرفوضة / Rejected alternatives

1. استبدال الجدولين فورًا بـBusinessPartner: مرفوض لخطر كسر المفاتيح والتاريخ.
2. نسخ الزبون والمورد إلى جدول ثالث مع استمرار ثلاثة مصادر حقيقة: مرفوض بسبب الازدواج والغموض.
3. استخدام الاسم أو الرقم الضريبي كهوية مشتركة: مرفوض لأنهما قابلان للتغيير أو الغياب أو التكرار المشروع.
4. تخزين رصيد قابل للتعديل داخل Customer: مرفوض لأن الذمم مصدرها القيود والمطابقات.

## الترحيل والرجوع / Migration and rollback

`CUS-005` لا يغيّر قاعدة البيانات. ترحيلات `CUS-010` ستكون إضافية فقط وتحافظ على الأعمدة والعلاقات الحالية.
يمكن الرجوع من واجهة الملف الجديدة مع إبقاء الأعمدة الإضافية غير مستخدمة؛ لا يحتاج الرجوع إلى حذف بيانات أو
إعادة كتابة مستند. أي توحيد مستقبلي يحتاج نسخة احتياطية، مصالحة، تحققًا مزدوجًا وخطة رجوع مستقلة.

## النتائج / Consequences

يستطيع ملف الزبون الاحترافي البدء بأمان، وتبقى دورة البيع الحالية متوافقة. سيبقى بعض التكرار المشروع بين ملفي
الزبون والمورد إلى أن تثبت حاجة الربط أو التوحيد. هذا ثمن مقصود لحماية التاريخ وتقليل نطاق الترحيل.
