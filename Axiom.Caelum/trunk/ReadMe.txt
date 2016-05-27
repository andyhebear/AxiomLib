-= How can I install Caelum for NeoAxis? =-

Just copy-Paste the repertories "Bin" and "Src" to your NeoAxis' "Game" repertory, Windows will ask you if you want to replace files, select yes. Then you must edit your Bin\Data\Definitions\EntitySystem.config to add these lines:

typeClassAssemblies
{
	...	
	assembly {file = Caelum.dll}
}
logicSystem
{
	systemClassesAssemblies
	{
		...
		assembly {file = Caelum.dll}
	}

	referencedAssemblies
	{
		...
		assembly {file = Caelum.dll}
	}

	usingNamespaces
	{
		..
		namespace {name = Caelum}
	}
}

You can also see Bin\Data\Definitions\EntitySystem.sample.txt for example (it's my Bin\Data\Definitions\EntitySystem.config file)
If you get somes bugs, first check if all caelum's files are set in Bin\Data\Caelum NAE's repertory and if Bin\Caelum.dll is present.



-= How can I integrate Caelum for NeoAxis in my map? =-

By default all types are precreated so you don't need to modify them in the resource Editor if you don't want to custom Caelum components' creation.
In the map Editor just add Caelum Manager entity, sets up as you want and enjoy ;)
It's also recommended to add a EXP2 fog entity (else horizon will be white :s).

Tips: For map Edition, night and day alternance can disturb you so just set time to 0 to stop the caelum's time.



-= I use an old version of NAE ( <0.59), can I use Caelum for NAE ? =-

Caelum for NeoAxis works with 0.58 and 0.57 versions, you just have to use Caelum.dll.v058 or Caelum.dll.v057 instead of Caelum.dll. So copy-paste the correct version of the dll and rename it to Caelum.dll.
If you use an older version of NeoAxis, it may not work but you can try with the 0.57 dll version



-= I get a strange bug what can I do? =-

Please report it on the NAE forum
http://www.neoaxisgroup.com/phpBB2/viewtopic.php?t=1104&start=0&postdays=0&postorder=asc&highlight=

Or send a mail to gegem31@hotmail.com



-= How can I uninstall Caelum for NeoAxis? =-

Just delete Bin\Data\Caelum directory, Src\Caelum directory, Bin\Caelum.dll file and restaure your Bin\Data\Definitions\EntitySystem.config (juste delete the added line).



