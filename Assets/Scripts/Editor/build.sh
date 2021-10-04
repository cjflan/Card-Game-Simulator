echo Clearning Up Build Directory
rm -rf ../../../../Card-Game-Simulator/build/

echo Starting Build Process
/Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -projectPath ../../../../Card-Game-Simulator -executeMethod BuildScript.PreformBuild
echo Ended Build Process