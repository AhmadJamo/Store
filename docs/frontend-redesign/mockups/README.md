# MiniStore Frontend Concept Mockups

> الحالة: Visual direction only
> التاريخ: 2026-11-24

هذه الصور مراجع لاتجاه التصميم وليست دليلاً على أن الشاشات منفذة أو مرتبطة بالـAPI. الهوية أصلية لـMiniStore: Plum للـshell، Teal للإجراءات، وأس surfaces محايدة. يُقاس التنفيذ الفعلي بالعقود والاختبارات والقبول البصري داخل التطبيق.

## الصور

1. `01-app-launcher.png` — شاشة التطبيقات الرئيسية مع 10 وحدات وRecent Work.
2. `02-products-list.png` — نمط List كثيف للمنتجات مع Search/Filters/Group By/Favorites وجدول خادمي.
3. `03-sales-order-record.png` — نمط Record/Form لمستند بيع مع status bar وsmart metrics وevidence panel.
4. `04-pos-workspace.png` — مساحة POS touch-first بمنتجات generic دون علامات تجارية أو حقول محاسبية خفية.

## قواعد الاستخدام

- لا تنسخ النصوص أو الأرقام الظاهرة كبيانات حقيقية.
- لا تنفذ زرًا أو view أو metric بلا capability وpermission وAPI موثقة.
- لا تعتبر القياسات pixel-perfect؛ تُحول إلى tokens ومكونات المشروع.
- النسخة العربية يجب أن تعكس الاتجاه باستخدام logical CSS دون تغيير ترتيب DOM المنطقي.
- الصور لا تستبدل مواصفات `04-UX-ARCHITECTURE.md` و`05-DESIGN-SYSTEM.md` و`06-NAVIGATION-AND-ROUTES.md`.
