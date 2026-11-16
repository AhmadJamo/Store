# تغطية وحدات ERP — baseline V3.0

| الوحدة | الحالة الحالية | الدليل | الخطوة |
|---|---|---|---|
| الهوية والصلاحيات | Implemented / needs audit | Identity + Permission policies + Angular users/roles | REC-003/004 |
| الشركات/التعددية | Partially Implemented | Tenant/SaaS/onboarding | model audit |
| الفروع | Partially Implemented | Branch workspace + hidden create + grants | REC-001 |
| المستودعات/WMS | Implemented; delete safety verified | Warehouses API/UI + migrated SQL delete-safety test | REC-003 coverage inventory |
| المنتجات | Implemented | product/catalog/recipes routes and services | functional audit |
| المشتريات | Broadly implemented | PR→sourcing→PO→receipt→bill/payments | functional audit |
| المبيعات | Broadly implemented | quotations→orders→delivery→invoice→payment/returns | functional audit |
| المحاسبة | Partially implemented enterprise scope | CoA/journals/reports/aging/close/bank reconciliation | reconciliation audit |
| POS | Implemented core / advanced V3 missing | POS sales + terminal experience + cash sessions | POS-001 |
| التدقيق | Implemented core | tenant/platform audit evidence + correlation IDs | POS event coverage audit |
| Offline | Missing/Requires ADR | لا outbox مالي مثبت | POS-002 |
| Reporting Studio | Partially Implemented | Analytics Center/report builder | REP-001 |
| Restaurant/Kitchen | Requires Investigation | recipes/prepared products موجودة؛ workflow غير مثبت | later audit |

الحالات تعكس الأدلة الحالية وليست شهادة اكتمال وظيفي شامل.
