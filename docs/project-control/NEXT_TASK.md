# المهمة التالية الوحيدة / Next Task

## `MOD-030E3E` — إدارة المستخدمين والأدوار / Users and Roles Cutover

> الحالة: APPROVED

اكتمل `MOD-030E3D`: أصبح tenant AuditEvents السطح السابع والعشرين AngularDefault بعد hosted ‏15/15 والقطع والرجوع
57/57. بقي 55 سطحًا mapped يعمل MVC افتراضيًا.

### النطاق المحدود

- Users وRoles فقط ضمن مصادقة الشركة الحالية.
- Users Index/Create GET إلى `/users` و`/users/new`.
- Roles Index/Create/Edit GET إلى `/roles` و`/roles/new` و`/roles/{id}/edit` مع معرّف موجب.
- كل POST/Delete وتعيين صلاحيات أو عضويات تبقى مستقلة بصلاحياتها وantiforgery وخارج كتالوج العرض.

### القبول

- كل GET مصرح يصل إلى مسار Angular الدقيق ويحافظ على query والمعرّف عند الحاجة.
- أي mutation أو معرّف غير موجب أو action غير مدعوم يبقى خارج presentation cutover.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا تغيير قاعدة بيانات.
