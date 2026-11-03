# المهمة التالية الوحيدة / Next Task

## `MOD-030E5V` — مصمم المستندات / Document Designs Cutover

> الحالة: APPROVED

اكتمل `MOD-030E5U`: أصبح Dashboard السطح 79 بعد hosted ‏15/15 والقطع/الرجوع 137/137 والبوابة
1,827 بلا عملية أعمال. بقي 3 أسطح MVC.

### النطاق المحدود

- تحويل DocumentDesigns Index وCreate وDetails GET إلى `/app/settings/document-designs` مع تحقق المعرّف.
- إبقاء Create POST وActivate وSetDefault وArchive تحت الصلاحيات والخدمات وantiforgery خارج التحويل.

### القبول

- GET المصرح يصل إلى Angular، والرجوع إلى MVC مثبت بلا إنشاء/تفعيل/افتراضي/أرشفة تصميم.
