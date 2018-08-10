
####################### 引用common #######################  
source "D:/Projects/XxcProjects/ShellScript/common.sh"

####################### 初始化 #######################  
	# 当前目录被定义在 $startScriptDir 中
alanInit
mydir=$startScriptDir
 
#######################        #######################   
if [ "$msbuild" == "" ] ; then
	showErrorAndExit "系统环境变量msbuild必须设置到MSBuild.exe的路径" 
fi  

rm -rf d:/temp/Log2Window
checkIfActionError "清空目录出错 $LINENO"  
rm -f d:/temp/Log2Window.zip
checkIfActionError "删除文件出错 $LINENO"  

mkdir -p d:/temp/Log2Window
mkdir -p d:/temp/Log2Window/ExampleProject
"$msbuild"  \
    //t:rebuild \
	/property:OutputPath=d:/temp/Log2Window \
	/property:Configuration=Debug \
	/property:DeployOnBuild=true \
	"Log2Window\Log2Window.csproj"
checkIfActionError "编译出错" 
 
cp -rf TestLog4net d:/temp/Log2Window/ExampleProject
checkIfActionError "出错 $LINENO" 
cp -rf TestNLog d:/temp/Log2Window/ExampleProject
checkIfActionError "出错 $LINENO" 

find d:/temp/Log2Window/ExampleProject -name "bin" -exec rm -rf {} \;
find d:/temp/Log2Window/ExampleProject -name "obj" -exec rm -rf {} \;
find d:/temp/Log2Window/ExampleProject -name "packages" -exec rm -rf {} \;   

mkdir d:/temp/Log2Window/bin
mv d:/temp/Log2Window/*.dll d:/temp/Log2Window/bin
mv d:/temp/Log2Window/*.xml d:/temp/Log2Window/bin

cd d:/temp
zip -r Log2Window.zip Log2Window
checkIfActionError "压缩出错" 
cp -af d:/temp/Log2Window/. "D:/Users/Alan/Documents/YunPan/Program Files/Log2Window" 
checkIfActionError "出错 $LINENO" 
showInfo 全部完成.

read temp