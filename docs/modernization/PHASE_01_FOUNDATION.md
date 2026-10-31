# MOD-001 — Safe Angular Development Foundation

> Status: COMPLETE  
> Started: 2026-10-24

## نطاق المرحلة

- حماية `master` بفرع `codex/angular-modernization-foundation`.
- تثبيت نقطة rollback عند `fd0deb7082bcc7c8cd5ae19061a33c300d6aad6e`.
- إنشاء Angular workspace مستقل باسم `MiniStore Front-End`.
- اعتماد Angular 22.2.4، Node 24.20.0، npm 11.19.0، standalone components، strict TypeScript، SCSS وlazy routing.
- إنشاء بنية `core/shell/workspace/shared/features` دون ملئها بتجريدات افتراضية.
- إضافة RTL/LTR foundation وdesign tokens وresponsive shell أولي.
- إضافة development proxy لنطاقي `/api` و`/Account` فقط.
- إضافة Workspace state contract صغير قائم على Signals لإثبات اتجاه الملكية، دون تنفيذ tabs/history الكامل قبل Phase 4.
- تشغيل Angular build/unit tests وbackend regression للتحقق من عدم كسر النظام الحالي.

## ما لا تتضمنه المرحلة

- لا API داخلي جديد.
- لا تغيير للمصادقة أو الصلاحيات.
- لا database migration.
- لا نقل لأي MVC screen.
- لا Angular Material أوTailwind أوAG Grid.
- لا NgRx.

## معايير القبول

- Angular production build ناجح.
- Angular unit tests ناجحة.
- backend Release build والانحدار ناجحان.
- Arabic RTL وEnglish LTR يعملان في shell.
- لا secrets أوlong-lived tokens في frontend.
- وثائق الخطة والقرار والـrollback محدثة.

## دليل التنفيذ

- Angular `npm run build`: نجح؛ initial bundle بلغ 229.81 kB raw و62.75 kB estimated transfer، مع lazy chunks للـshell والصفحة الرئيسية.
- Angular `npm test -- --watch=false`: نجح 1/1 test file و1/1 test.
- Backend Release build: نجح دون warnings أوerrors.
- Backend regression: نجح 1066 فحصًا.
- Database impact: لا يوجد.
- Backend runtime impact: لا يوجد.
