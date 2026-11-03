# المهمة التالية الوحيدة / Next Task

## `MOD-030E5P` — نظرة منصة الإدارة وعملياتها / Platform Overview and Management Cutover

> الحالة: APPROVED

اكتمل `MOD-030E5O`: أصبح PlatformAccount السطح 73 بعد hosted ‏15/15 والقطع/الرجوع المجهول 132/132
والبوابة 1,786 بلا محاولة اعتماد أو تغيير جلسة. بقي 9 أسطح MVC.

### النطاق المحدود

- تحويل Platform Index وCompanies وPendingPayments وPlans وCreatePlan وPromotions وCreatePromotion GET إلى
  `/app/platform-control` و`/app/platform-management`.
- إبقاء تحديث الشركة وإنشاء الخطة/العرض وتأكيد الدفع تحت سياسات Platform وantiforgery وخارج تحويل العرض.

### القبول

- GET بجلسة Platform مصرح بها يصل إلى Angular مع حفظ الاستعلام، والرجوع إلى MVC مثبت بلا تغيير شركة أو خطة أو عرض أو دفعة.
