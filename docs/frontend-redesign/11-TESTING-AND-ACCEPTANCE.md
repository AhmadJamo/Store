# 11 — استراتيجية الاختبار والقبول

> الحالة: Active acceptance authority
> آخر تحديث: 2026-11-24

## 1. طبقات التحقق

| الطبقة | ما تثبته | ما لا تثبته |
|---|---|---|
| Unit/Component | العرض، الحالات، keyboard contracts، خدمات UI | تكامل خادم حي |
| Static accessibility | أسماء الأزرار، IDs، shell landmarks، feedback semantics | تجربة قارئ شاشة كاملة |
| Production build | TypeScript/templates/budgets/error limits | صحة business workflow |
| Browser presentation | RTL/LTR، theme/density، App Switcher، keyboard، responsive، screenshots | بيانات أو صلاحيات حية عند synthetic session |
| Authenticated integration | Route/Page/API وpermission/branch scope | أثر مالي/مخزني بلا فحص domain evidence |
| Workflow/E2E | العملية من البداية للنهاية وحالات الفشل | لا يعوض اختبارات Application/Domain |
| Visual regression | اختلاف التصميم عبر viewport/theme/locale | لا يثبت صحة الأرقام أو الانتقالات |

## 2. بوابة Design System المنفذة

التاريخ: 2026-11-24.

- Angular: 123 test files، 260/260 tests passed.
- Angular production build: passed.
- Static accessibility: 138 templates passed.
- UI/UX consistency: 121 feature pages accounted for.
- AngularBrowserAcceptance build: 0 warnings، 0 errors.
- Playwright presentation: 6 passed، 0 failed، 3 skipped.
- Passed: bounded UI session، shared states، RTL↔LTR، keyboard skip، Dark/Compact + Light/Comfortable، Desktop + 390px responsive وmobile menu.
- Skipped intentionally: authenticated inventory/API session checks لعدم تمرير API URL أو external authenticated state. لا يوجد ادعاء API parity في هذه البوابة.

الأدلة محفوظة في `acceptance/design-system-2026-11-24/`.

### بوابة FRD-E الحالية

شريحة App Switcher: 124 ملف اختبار و264/264 اختبار، production build، 139 قالب وصول، 121 صفحة اتساق، وPlaywright ‏7 passed/0 failed/3 authenticated-API skips. أضيف فحص App Switcher على Desktop و390px بجلسة عرض محدودة إلى Products.View؛ هذا يثبت تصفية العرض ولا يدّعي تكامل API حي.

## 3. بوابة كل migration slice

1. تحليل old page actions والpermissions والـAPI calls.
2. اختبارات component للحالات والأفعال الجديدة.
3. production build وstatic accessibility وUI consistency.
4. route/direct-link/back/refresh/dirty behavior.
5. browser evidence لـEN/LTR وAR/RTL وdesktop/tablet/mobile.
6. authenticated integration للقراءة والكتابة المعنية.
7. business evidence للأثر المالي/المخزني إن وجد.
8. parity record وrollback قبل إزالة القديم.

## 4. حالات إلزامية

Loading، empty، filtered empty، validation، HTTP failure، conflict/concurrency، duplicate click، expired session، unsaved changes، restricted branch، permission denied، large data set وresponsive reflow.

## 5. Visual baseline policy

- تحفظ baseline فقط بعد مراجعة بشرية للصورة الفعلية.
- screenshot success لا يغلق الوظيفة إذا كان layout قبيحاً أو مضللاً.
- أي تغيير مقصود يوثق سببه ويحدث baseline في commit نفسه.
- الصور المولدة في `mockups/` اتجاه تصميم وليست baseline.
- لقطات `acceptance/` دليل للنسخة المبنية في التاريخ المذكور.

## 6. معايير الفشل

تفشل البوابة عند broken route، console error غير معالج في السيناريو، overflow غير مقصود، inaccessible action، hard-coded presentation text، permission mismatch، API contract mismatch غير موثق، أو اختلاف أثر business عن الخادم.
