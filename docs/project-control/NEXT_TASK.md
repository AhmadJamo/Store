# المهمة التالية الوحيدة / Next Task

## `MOD-030E5O` — دخول منصة الإدارة / Platform Sign-in Cutover

> الحالة: APPROVED

اكتمل `MOD-030E5N`: أصبح Public السطح 72 بعد hosted ‏15/15 والقطع/الرجوع المجهول 131/131 والبوابة
1,782 بلا تسجيل أو تغيير خطة/اشتراك. بقي 10 أسطح MVC.

### النطاق المحدود

- تحويل PlatformAccount Login GET المجهول إلى `/app/platform-login` مع حفظ returnUrl ومؤشرات rate limit.
- إبقاء Login POST، التحقق من الاعتماد، rate limiting، إصدار cookie وLogout POST خارج تحويل العرض.

### القبول

- GET المجهول يصل إلى Angular مع حفظ الاستعلام، والرجوع إلى MVC مثبت بلا محاولة دخول أو تغيير جلسة Platform.
