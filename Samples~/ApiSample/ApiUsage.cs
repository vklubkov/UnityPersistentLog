using System;
using PersistentLog;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Samples.Persistent_Log._1._0._0.API_Usage {
    public static class ApiUsage {
        // Typical operations that trigger domain reload.
        enum DomainReloadTrigger {
            None,
            AssetDatabaseRefresh,
            RequestScriptCompilation,
            RequestScriptReload,
        }

        const string _packageName = "Persistent Log Sample";
        const string _logFilePath = "Logs/PersistentLogSample.log";

        static bool PrepareOperation(bool fail) {
            // Some operations may have a preparation phase
            // that should be in the dedicated log IF the operation runs
            // but should NOT be there if the preparation phase fails.

            LogInterceptor.Awake(_packageName);
            try {
                Debug.Log("Operation is preparing...");
                if (fail)
                    throw new InvalidOperationException("Operation preparation failed.");

                return true;
            }
            catch (Exception e) {
                Debug.LogException(e);
                LogInterceptor.Stop(_packageName);
                return false;
            }
        }

        static void RunOperation(bool fail, DomainReloadTrigger domainReloadTrigger) {
            // Start intercepting and open the window
            LogInterceptor.Start(
                _packageName,
                _logFilePath,
                enableAutomatedLogFile: true,
                automaticallyOpenLogWindow: true);

            try {

                Debug.Log("Operation started...");

                // Note that these operations are not immediate and they execute asynchronously.
                // So you will see the "Operation finished." and "Operation failed." log messages
                // in the log window before the domain reloading starts. But this is enough to
                // test how the window itself survives the domain reload.

                switch (domainReloadTrigger) {
                    case DomainReloadTrigger.None:
                        break;
                    case DomainReloadTrigger.AssetDatabaseRefresh:
                        AssetDatabase.Refresh(
                            ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
                        break;
                    case DomainReloadTrigger.RequestScriptCompilation:
                        CompilationPipeline.RequestScriptCompilation();
                        break;
                    case DomainReloadTrigger.RequestScriptReload:
                        EditorUtility.RequestScriptReload();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(domainReloadTrigger), domainReloadTrigger, null);
                }

                if (fail)
                    throw new InvalidOperationException("Operation failed.");

                Debug.Log("Operation finished.");
            }
            catch (Exception e) {
                Debug.LogException(e);
            }
            finally {
                // Stop intercepting
                LogInterceptor.Stop(_packageName);
            }
        }

#region Menu

        // Window

        [MenuItem("Tools/Persistent Log Sample/" + "Show Log Window", priority = 1000)]
        public static void ShowLogWindow() =>
            LogInterceptor.ShowWindow(_packageName, _logFilePath);


        // Preparation

        [MenuItem(
            "Tools/Persistent Log Sample/" +
            "{ Preparation: Success, Operation: None, Domain reload: None }",
            priority = 1011)]
        public static void PreparationSuccessOperationNoneDomainReloadNone() {
            if (PrepareOperation(fail: false))
                LogInterceptor.Stop(_packageName); // Cleanup
        }

        [MenuItem(
            "Tools/Persistent Log Sample/" +
            "{ Preparation: Failure, Operation: None, Domain reload: None }",
            priority = 1012)]
        public static void PreparationFailureOperationNoneDomainReloadNone() {
            if (PrepareOperation(fail: true))
                LogInterceptor.Stop(_packageName); // Cleanup (should never be here)
        }


        // No domain reload

        [MenuItem(
            "Tools/Persistent Log Sample/" +
            "{ Preparation: None, Operation: Success, Domain reload: None }",
            priority = 1023)]
        public static void PreparationNoneOperationSuccessDomainReloadNone() =>
            RunOperation(fail: false, DomainReloadTrigger.None);

        [MenuItem(
            "Tools/Persistent Log Sample/" +
            "{ Preparation: Success, Operation: Success, Domain reload: None }",
            priority = 1024)]
        public static void PreparationSuccessOperationSuccessDomainReloadNone() {
            if (PrepareOperation(fail: false))
                RunOperation(fail: false, DomainReloadTrigger.None);
        }

        [MenuItem(
            "Tools/Persistent Log Sample/" +
            "{ Preparation: None, Operation: Failure, Domain reload: None }",
            priority = 1025)]
        public static void PreparationNoneOperationFailureDomainReloadNone() =>
            RunOperation(fail: true, DomainReloadTrigger.None);

        [MenuItem(
            "Tools/Persistent Log Sample/" +
            "{ Preparation: Success, Operation: Failure, Domain reload: None }",
            priority = 1026)]
        public static void PreparationSuccessOperationFailureDomainReloadNone() {
            if (PrepareOperation(fail: false))
                RunOperation(fail: true, DomainReloadTrigger.None);
        }


        // AssetDatabase.Refresh

        [MenuItem(
            "Tools/Persistent Log Sample/" +
            "{ Preparation: None, Operation: Success, Domain reload: AssetDatabase.Refresh }",
            priority = 1037)]
        public static void PreparationNoneOperationSuccessDomainReloadAssetDatabaseRefresh() =>
            RunOperation(fail: false, DomainReloadTrigger.AssetDatabaseRefresh);

        [MenuItem(
            "Tools/Persistent Log Sample/" +
            "{ Preparation: Success, Operation: Success, Domain reload: AssetDatabase.Refresh }",
            priority = 1038)]
        public static void PreparationSuccessOperationSuccessDomainReloadAssetDatabaseRefresh() {
            if (PrepareOperation(fail: false))
                RunOperation(fail: false, DomainReloadTrigger.AssetDatabaseRefresh);
        }

        [MenuItem(
            "Tools/Persistent Log Sample/" +
            "{ Preparation: None, Operation: Failure, Domain reload: AssetDatabase.Refresh }",
            priority = 1039)]
        public static void PreparationNoneOperationFailureDomainReloadAssetDatabaseRefresh() =>
            RunOperation(fail: true, DomainReloadTrigger.AssetDatabaseRefresh);

        [MenuItem(
            "Tools/Persistent Log Sample/" +
            "{ Preparation: Success, Operation: Failure, Domain reload: AssetDatabase.Refresh }",
            priority = 1040)]
        public static void PreparationSuccessOperationFailureDomainReloadAssetDatabaseRefresh() {
            if (PrepareOperation(fail: false))
                RunOperation(fail: true, DomainReloadTrigger.AssetDatabaseRefresh);
        }


        // RequestScriptCompilation

        [MenuItem(
            "Tools/Persistent Log Sample/" +
            "{ Preparation: None, Operation: Success, Domain reload: RequestScriptCompilation }",
            priority = 1051)]
        public static void PreparationNoneOperationSuccessDomainReloadRequestScriptCompilation() =>
            RunOperation(fail: false, DomainReloadTrigger.RequestScriptCompilation);

        [MenuItem(
            "Tools/Persistent Log Sample/" +
            "{ Preparation: Success, Operation: Success, Domain reload: RequestScriptCompilation }",
            priority = 1052)]
        public static void PreparationSuccessOperationSuccessDomainReloadRequestScriptCompilation() {
            if (PrepareOperation(fail: false))
                RunOperation(fail: false, DomainReloadTrigger.RequestScriptCompilation);
        }

        [MenuItem(
            "Tools/Persistent Log Sample/" +
            "{ Preparation: None, Operation: Failure, Domain reload: RequestScriptCompilation }",
            priority = 1053)]
        public static void PreparationNoneOperationFailureDomainReloadRequestScriptCompilation() =>
            RunOperation(fail: true, DomainReloadTrigger.RequestScriptCompilation);

        [MenuItem(
            "Tools/Persistent Log Sample/" +
            "{ Preparation: Success, Operation: Failure, Domain reload: RequestScriptCompilation }",
            priority = 1054)]
        public static void PreparationSuccessOperationFailureDomainReloadRequestScriptCompilation() {
            if (PrepareOperation(fail: false))
                RunOperation(fail: true, DomainReloadTrigger.RequestScriptCompilation);
        }


        // RequestScriptReload

        [MenuItem(
            "Tools/Persistent Log Sample/" +
            "{ Preparation: None, Operation: Success, Domain reload: RequestScriptReload }",
            priority = 1065)]
        public static void PreparationNoneOperationSuccessDomainReloadRequestScriptReload() =>
            RunOperation(fail: false, DomainReloadTrigger.RequestScriptReload);

        [MenuItem(
            "Tools/Persistent Log Sample/" +
            "{ Preparation: Success, Operation: Success, Domain reload: RequestScriptReload }",
            priority = 1066)]
        public static void PreparationSuccessOperationSuccessDomainReloadRequestScriptReload() {
            if (PrepareOperation(fail: false))
                RunOperation(fail: false, DomainReloadTrigger.RequestScriptReload);
        }

        [MenuItem(
            "Tools/Persistent Log Sample/" +
            "{ Preparation: None, Operation: Failure, Domain reload: RequestScriptReload }",
            priority = 1067)]
        public static void PreparationNoneOperationFailureDomainReloadRequestScriptReload() =>
            RunOperation(fail: true, DomainReloadTrigger.RequestScriptReload);

        [MenuItem(
            "Tools/Persistent Log Sample/" +
            "{ Preparation: Success, Operation: Failure, Domain reload: RequestScriptReload }",
            priority = 1068)]
        public static void PreparationSuccessOperationFailureDomainReloadRequestScriptReload() {
            if (PrepareOperation(fail: false))
                RunOperation(fail: true, DomainReloadTrigger.RequestScriptReload);
        }

#endregion
    }
}