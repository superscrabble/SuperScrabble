import { Component } from '@angular/core';
import { AngularFireRemoteConfig, budget, filterFresh, scanToObject } from '@angular/fire/compat/remote-config';
import { traceUntilFirst } from '@angular/fire/performance';
import { fetchAndActivate, getAll, getAllChanges, getRemoteConfig, getValue, RemoteConfig, RemoteConfigInstances, RemoteConfigModule } from '@angular/fire/remote-config';
import { Router } from '@angular/router';
import { fetchConfig } from 'firebase/remote-config';
import { EMPTY, filter, first, last, map, Observable, scan, tap } from 'rxjs';
import { AppConfig } from './app-config';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'super-scrabble-app';
  
  //readonly change$: Observable<any>;

  //constructor(remoteConfig: RemoteConfig, router: Router) {
  constructor(remoteConfig: AngularFireRemoteConfig, router: Router) {
    /*
    console.log(Date.now());
    remoteConfig.getValue("")

    AppConfig.isRemoteConfigFetched = false;
    remoteConfig.fetchAndActivate().then(hasActivatedTheFetch => {
      console.log("READY FOR WORK");
      remoteConfig.getAll().then(all => {
        console.log("After ready");
        console.log(all);
        AppConfig.isRemoteConfigFetched = true;
        
      })
    })

    remoteConfig.changes.pipe(
      filterFresh(172_800_000), // ensure we have values from at least 48 hours ago
      first(),
      // scanToObject when used this way is similar to defaults
      // but most importantly smart-casts remote config values and adds type safety
      // scanToObject({
      //   enableAwesome: true,
      //   titleBackgroundColor: 'blue',
      //   titleFontSize: 12
      // })
    ).subscribe(value => {
      console.log("AFTER FIRST FIRST FETCH");
      console.log(value);
    });

    remoteConfig.strings.subscribe((data) => {
      console.log("STRING SUBSCRIBING")
      console.log(data)
    })
    remoteConfig.booleans.subscribe(); // as booleans
    remoteConfig.numbers.subscribe(); // as numbers
  
    // however those may emit more than once as the remote config cache fires and gets fresh values
    // from the server. You can filter it out of .changes for more control:
    remoteConfig.changes.pipe(
      map(param => param.asString()),
      // budget at most 800ms and return the freshest value possible in that time
      // our budget pipe is similar to timeout but won't error or abort the pending server fetch
      // (it won't emit it, if the deadline is exceeded, but it will have been fetched so can use the
      // freshest values on next subscription)
      budget(800),
      last()
    ).subscribe(value => {
      console.log("NEW")
      console.log(value);
    })
  
    // just like .changes, but scanned into an array
    remoteConfig.parameters.subscribe(all => {
      console.log("AFTER PARAMETERS: " + all.length);
      console.log(all);
      AppConfig.remoteConfigData = new Map<string, string>(all.map(obj => [obj.key, obj._value]));
    });
  
    // or make promisified firebase().remoteConfig() calls direct off AngularFireRemoteConfig
    // using our proxy
    remoteConfig.getAll().then(all => {
      console.log("AFTER GET ALL: ");
      console.log(all);
    });
    remoteConfig.lastFetchStatus.then(status => {
      console.log("REMOTE CONFIG STATUS: " + status)
    });*/

    // fetchConfig(remoteConfig).then(() => {
    //   console.log("FETCHED");
    //   console.log(getAll(remoteConfig));
    // })

    // fetchAndActivate(remoteConfig)
    //   .then((value) => {
    //     console.log("Not error")
    //     console.log(value)
    //     if(value) {
    //       console.log(getValue(remoteConfig, "testProp").asString())
    //       console.log(getValue(remoteConfig, "secondTest").asString())
    //     }
    //     console.log(getAll(remoteConfig));
    //     let values = getAll(remoteConfig);

    //     console.log("Start of map")
      
    //     /*map(values, x => {
    //       console.log(x.asString());
    //     });*/
    //     console.log("End of map")
        
    //     console.log(values['testProp'].asString())
    //     console.log(values['secondTest'].asString());
    //     console.log(values['LoginBtnText'].asString());
    //     console.log(getValue(remoteConfig, "testProp").asString())
    //     console.log(getValue(remoteConfig, "secondTest").asString())
    //   })
    //   .catch((err) => {
    //     console.log("ERROR ON FETCHING")
    //     console.log(err);   
    // });
    // console.log("IN the constructor")
    // if (remoteConfig) {
    //   console.log("CONSTR MAIN PART")
    //   this.change$ = getAllChanges(remoteConfig).pipe(
    //     traceUntilFirst('testProp'),
    //     tap(it => console.log('REMOTE CONFIG', it)),
    //   );
    //   console.log("CONSTR MAIN PART")
    //   console.log(this.change$);
    //   let value = getValue(remoteConfig, "testProp")
    //   console.log(value)
    // } else {
    //   this.change$ = EMPTY;
    // }
    // console.log("BOTTOM the constructor")
    // console.log(this.change$);
    
    
  }
}
