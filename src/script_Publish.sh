
####################### 引用common #######################  
source "/mnt/d/Projects/XxcProjects/ShellScript/common.sh"

####################### 初始化 #######################  
	# 当前目录被定义在 $startScriptDir 中
alanInit
mydir=$startScriptDir
 
#######################        #######################   
readVarFromNT msbuild
toBashPath msbuild
"$msbuild"

if [ "$msbuild" == "" ] ; then
	showErrorAndExit "系统环境变量msbuild必须设置到MSBuild.exe的路径" 
fi  

rm -rf /mnt/d/temp/Log2Window
checkIfActionError "清空目录出错 $LINENO"  
rm -f /mnt/d/temp/Log2Window.zip
checkIfActionError "删除文件出错 $LINENO"  

mkdir -p /mnt/d/temp/Log2Window
mkdir -p /mnt/d/temp/Log2Window/ExampleProject
"$msbuild"  \
    /t:rebuild \
	/property:OutputPath=d:/temp/Log2Window \
	/property:Configuration=Release \
	/property:DeployOnBuild=true \
	"Log2Window\Log2Window.csproj"
checkIfActionError "编译出错" 
 
cp -rf TestLog4net /mnt/d/temp/Log2Window/ExampleProject
checkIfActionError "出错 $LINENO" 
cp -rf TestNLog /mnt/d/temp/Log2Window/ExampleProject
checkIfActionError "出错 $LINENO" 

find /mnt/d/temp/Log2Window/ExampleProject -name "bin" -exec rm -rf {} \;
find /mnt/d/temp/Log2Window/ExampleProject -name "obj" -exec rm -rf {} \;
find /mnt/d/temp/Log2Window/ExampleProject -name "packages" -exec rm -rf {} \;   

mkdir /mnt/d/temp/Log2Window/bin
mv /mnt/d/temp/Log2Window/*.dll /mnt/d/temp/Log2Window/bin
mv /mnt/d/temp/Log2Window/*.xml /mnt/d/temp/Log2Window/bin

cd /mnt/d/temp
zip -r Log2Window.zip Log2Window
checkIfActionError "压缩出错" 
cp -af /mnt/d/temp/Log2Window/. "/mnt/d/Users/Alan/Documents/YunPan/Program Files/Log2Window" 
checkIfActionError "出错 $LINENO" 
showInfo 全部完成.

read temp