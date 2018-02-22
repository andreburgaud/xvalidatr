default: help

help:
	@echo 'Usage:'
	@echo '    make clean           Delete temporaary files (obj and exe)'
	@echo '    make dist            create distribution (zip file)'
	@echo '    make release         Build release executable'
	@echo '    make debug           Build debug executable'
	@echo

clean:
	msbuild /t:Clean
	rm -rf dist
	rm XvalidatR_*.zip || true

debug:
	msbuild

dist: clean release
	cp xvalidatr bin/x64/Release
	mkdir dist
	cp xvalidatr dist
	cp bin/x64/Release/xvalidatr.exe dist
	zip -j XvalidatR_1.3.0.zip dist/*

release:
	msbuild /p:Configuration=Release



