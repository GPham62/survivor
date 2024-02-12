@ECHO "waiting.."
@ECHO OFF
SET fileinput=%~1
SET fileoutput=%fileinput:aab=apk%
java -jar bundletool-all-0.14.0.jar build-apks --bundle="%fileinput%" --output tmp.apks --ks=G24New.keystore --ks-pass=pass:Tmadmin12 --ks-key-alias=com.wm.games.hero.survivor --key-pass=pass:Tmadmin12 --mode=universal
jar xf tmp.apks
del "%fileoutput%"
copy universal.apk "%fileoutput%"
del universal.apk
del tmp.apks
@ECHO "done!!!"
PAUSE