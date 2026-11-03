# المهمة التالية الوحيدة / Next Task

## `MOD-030E5S` — واجهات حساب المستخدم / Tenant Account Presentation Cutover

> الحالة: APPROVED

اكتمل `MOD-030E5R`: أصبح PlatformAuditRetention السطح 76 بعد hosted ‏15/15 والقطع/الرجوع 132/132
والبوابة 1,810 مع إثبات عزل Owner، بلا تغيير تدقيق. بقي 6 أسطح MVC.

### النطاق المحدود

- تحويل Account Login وRegister وAccessDenied GET إلى `/app/login` و`/app/register` و`/app/access-denied`.
- إبقاء Login/Register/Logout POST والتحقق من الهوية وإنشاء الشركة والجلسة خارج تحويل العرض.

### القبول

- GET المجهول يحفظ returnUrl والخطة المختارة، والرجوع إلى MVC مثبت بلا محاولة دخول أو تسجيل أو تغيير جلسة.
