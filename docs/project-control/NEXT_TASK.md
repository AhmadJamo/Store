# المهمة التالية الوحيدة / Next Task

## `MOD-030E5T` — إعداد الشركة الموجّه / Company Onboarding Cutover

> الحالة: APPROVED

اكتمل `MOD-030E5S`: أصبح Account السطح 77 بعد hosted ‏15/15 والقطع/الرجوع المجهول 135/135 والبوابة
1,820 بلا محاولة دخول أو تسجيل أو تغيير جلسة. بقي 5 أسطح MVC.

### النطاق المحدود

- تحويل CompanyOnboarding Index GET إلى `/app/onboarding` مع حفظ الاستعلام.
- إبقاء Complete وSkip وإنشاء بيانات البداية تحت `ICompanyOnboardingService` وantiforgery وخارج التحويل.

### القبول

- GET المصرح يصل إلى Angular، والرجوع إلى MVC مثبت بلا إكمال/تخطي أو إنشاء بيانات إعداد.
