# سجل القرارات / Decision Register

| القرار | الحالة | المرجع | الأثر |
|---|---|---|---|
| بنية طبقية وتطبيق موحد مقسم | Accepted (معتمد) | `ADR-001-layered-application.md` | لا microservices دون ADR جديد |
| EF + Identity + permissions | Accepted (معتمد) | `ADR-002-ef-identity-permissions.md` | الهوية الحالية محفوظة |
| Repository + UnitOfWork | Accepted with inconsistency (معتمد مع تفاوت) | `ADR-003-ef-repository-unit-of-work.md` | توحيد تدريجي لا إعادة كتابة |
| عزل شركة في قاعدة مشتركة | Accepted (معتمد) | قرارات 2026-09-14/15 | TenantId وعلاقات مركبة إلزامية |
| مخزون AVCO وحركة فعلية إضافية | Accepted transition (انتقال معتمد) | قرارات WMS | لا cutover بلا مصالحة |
| دورة شراء إضافية وحفظ التاريخ | Accepted (معتمد) | قرارات 2026-10-08..15 | Purchase القديم تاريخي |
| إيقاف إنشاء الشراء المباشر القديم | Accepted (معتمد) | 2026-10-15 | طلب شراء هو المدخل الجديد |
| Sale/POS القديم بيع فوري | Accepted (معتمد) | SAL-005 | لا يعاد تفسير التاريخ |
| COGS عند التسليم للمبيعات الجديدة | Accepted (معتمد) | SAL-005 | التسليم يملك خروج المخزون |
| التسليم يستهلك الحجز والموقع الدقيق جزئيًا/كليًا ويثبت الكلفة مرة واحدة | Accepted and implemented (معتمد ومنفذ) | `2026-10-20-sales-delivery-reservation-cost-boundary.md`, SAL-060 | OnHand وReserved ينخفضان ذريًا؛ لا فاتورة أو تحصيل؛ عكس المرحّل لمسار مرتجع مضبوط |
| فاتورة الزبون تفصل الاعتماد عن الترحيل والتحصيل وتُعكس بقيد مقابل | Accepted and implemented (معتمد ومنفذ) | `2026-10-21-customer-invoice-accounting-posting.md`, SAL-080 | قيد ذمم/إيراد/ضريبة بأبعاد المصدر؛ لا مخزون أو دفع؛ التحصيل SAL-090 |
| تسعير مبيعات حتمي وقابل للتفسير | Accepted and implemented (معتمد ومنفذ) | `ADR-20261019-DETERMINISTIC-SALES-PRICING-TAX-UOM.md`, SAL-020B | الأولوية والوحدة والصرف والتقريب والضريبة ثابتة؛ المستندات القادمة تنسخ اللقطات ولا تعيد حساب التاريخ |
| BusinessPartner موحد | Deferred (مؤجل) | CUS-000 | يتطلب CUS-005 ولا يبدل المعرفات فجأة |
| محرك اعتماد عام | Deferred (مؤجل) | Purchase alignment/FND | يحتاج حالة ثانية مثبتة |
| outbox عام أو broker | Deferred (مؤجل) | FND-000 | supplier outbox يبقى محدودًا |
| RLS/Redis/stamps/microservices | Unresolved ADR triggers (قرارات مستقبلية) | ERP SaaS foundation | لا تعتمد دون قياس وحاجة |
| مزود الدفع الخارجي | Unresolved (غير محسوم) | SaaS docs | يلزم ADR وأمان webhook |
| RPO/RTO وسياسة النسخ | Unresolved P0 (غير محسوم حرج) | FND-010 | أول أساس تشغيلي بعد السر |
| حدث تدقيق دلالي إضافي مع حفظ السجل القديم | Accepted; foundation + first coverage implemented (معتمد؛ الأساس وأول تغطية منفذان) | `2026-10-15-audit-event-v2-boundary.md` | Application يكتب السياق ذريًا؛ القيود والحركات والتواريخ تبقى مصادر الحقيقة |
| دفعات الزبائن وأرصدة الفواتير | Accepted and implemented (معتمد ومنفذ) | `2026-10-21-customer-payment-allocation.md` | الدفعة مستند مستقل؛ الرصيد مشتق من التوزيعات المرحلة؛ العكس يسبق عكس الفاتورة |

أي قرار جديد أو تعديل قرار معتمد يحتاج ADR (سجل قرار معماري) ولا يحل بصمت داخل كود مهمة أخرى.
