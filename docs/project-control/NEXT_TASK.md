# المهمة التالية الوحيدة / Next Task

## `MOD-030E3B` — تعريفات كتالوج المنتجات / Product Catalog Definitions Cutover

> الحالة: APPROVED

اكتمل `MOD-030E3A`: أصبحت Branches وPaymentMethods وTaxRates المجموعة المشتركة الأولى في الكتالوج، بعد hosted
‏15/15 والقطع والرجوع 52/52. أصبح 23 سطحًا AngularDefault وبقي 59 سطحًا mapped يعمل MVC افتراضيًا.

### النطاق المحدود

- ProductCategories وProductAttributes فقط.
- يضاف ProductCategories Index GET إلى `/products/categories`.
- يضاف ProductAttributes Index GET إلى `/products/attributes`، وValues GET ذو `productId` الموجب إلى
  `/products/{id}/attributes`.
- Create وSetActive وValues POST تبقى أوامر API/MVC مستقلة بصلاحياتها وantiforgery، ولا تدخل كتالوج العرض.

### الهدف

- تدقيق أن مسارات Angular تعرض تعريفات التصنيفات والخصائص وقيم المنتج بالصلاحيات الحالية.
- إضافة خرائط GET الدقيقة وتفعيل السطحين AngularDefault كدفعة تشغيلية واحدة.
- توسيع مشغل cutover المصرح، ثم إثبات AngularDefault وprocess-local MVC rollback.
- إبقاء Controllers وRazor كاملة كمسار رجوع.

### القبول

- كل GET مصرح يصل إلى مسار Angular الدقيق ويحافظ على query والمعرّف الموجب عند الحاجة.
- المعرّف المفقود/صفر/السالب، وأي action كتابة أو طلب غير GET/HEAD، يبقى خارج presentation cutover.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا تغيير قاعدة بيانات.
