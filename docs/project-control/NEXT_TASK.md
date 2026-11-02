# المهمة التالية الوحيدة / Next Task

## `MOD-030E4Y` — أوامر البيع وحجوزاتها / Sales Orders and Reservations Cutover

> الحالة: APPROVED

اكتمل `MOD-030E4X`: أصبحت SalesQuotations السطح 56 بعد hosted ‏15/15 ومجموعة القطع/الرجوع المصرح بها 104/104 والبوابة 1,663. لم تحمل الجلسة صلاحيات عروض الأسعار ولم يتغير عرض. بقي 26 سطحًا MVC.

### النطاق المحدود

- SalesOrders Index وDetails وReserve ذوا المعرّف الموجب إلى `/sales/orders` و`/sales/orders/{id}` و`/sales/orders/{id}/reservations`.
- الحفاظ على status/search في القائمة.
- CreateFromQuotation وConfirm/Cancel وقرارات الموافقة وReserve/ReleaseReservation POST تبقى خارج التحويل.

### القبول

- كل GET مصرح يصل إلى مسار Angular الدقيق ويحافظ على query والمعرّف الموجب.
- كل mutation تبقى خارج presentation cutover بصلاحياتها وantiforgery.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا إنشاء أو تغيير أمر/حجز.
