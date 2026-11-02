# المهمة التالية الوحيدة / Next Task

## `MOD-030E5D` — استلامات مرتجعات العملاء / Customer Return Receipts Cutover

> الحالة: APPROVED

اكتمل `MOD-030E5C`: أصبحت CustomerReturnRequests السطح 61 بعد hosted ‏15/15 والقطع/الرجوع 104/104 والبوابة 1,707 بلا تغيير بيانات. بقي 21 سطحًا MVC.

### النطاق المحدود

- تحويل Index/Create/Details GET الآمنة لاستلامات مرتجعات العملاء.
- إبقاء Create/Inspect/Post/Cancel POST عبر Angular API المحمي خارج تحويل العرض.

### القبول

- GET المصرح يصل إلى Angular، وكل mutation تبقى محمية، والاختبارات تبقى خضراء بلا تغيير مخزون أو محاسبة.
