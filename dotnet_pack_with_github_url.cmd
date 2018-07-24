@echo OFF
FOR /F "tokens=*" %%a in ('git ls-remote --get-url origin') do SET GithubRepoUrl=%%a
SET GithubRepoUrl=%GithubRepoUrl:git@github.com:=https://github.com/%
SET GithubRepoUrl=%GithubRepoUrl:.git=%
cd ..
dotnet pack -c Release -o %LocalNugetPackages% --include-source --include-symbols
pause	