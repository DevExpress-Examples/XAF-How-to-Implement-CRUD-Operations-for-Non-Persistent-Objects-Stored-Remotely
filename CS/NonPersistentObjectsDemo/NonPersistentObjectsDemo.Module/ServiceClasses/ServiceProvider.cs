using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;

namespace NonPersistentObjectsDemo.Module {

    //public class CurrentUserServiceProvider<T> {
    //    private static Func<T> factory;
    //    public static void AddService(Func<T> factory) {
    //        CurrentUserServiceProvider<T>.factory = factory;
    //    }
    //    public static T GetService() {
    //        var vm = ValueManager.GetValueManager<T>(typeof(CurrentUserServiceProvider<T>).FullName);
    //        T result;
    //        if(vm.CanManageValue) {
    //            result = vm.Value;
    //            if(Equals(result, default(T))) {
    //                result = factory.Invoke();
    //                vm.Value = result;
    //            }
    //        }
    //        else {
    //            result = factory.Invoke();
    //        }
    //        return result;
    //    }
    //}

    //public class GlobalServiceProvider<T> {
    //    private static Func<T> factory;
    //    private static T instance;
    //    public static void AddService(Func<T> factory) {
    //        GlobalServiceProvider<T>.factory = factory;
    //    }
    //    public static T GetService() {
    //        typeof(T).TypeInitializer.Invoke(null, null);
    //        if(Equals(instance, default(T))) {
    //            instance = factory.Invoke();
    //        }
    //        return instance;
    //    }
    //}
}
