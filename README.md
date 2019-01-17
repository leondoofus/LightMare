# LightMare

## Levels
All levels' configuration files are in .\Optics\optics\Assets\Levels under the form:

element x y rotation

EOF


All elements use default configuration, to add more propeties, see more .\Optics\optics\Assets\Systems\LoadingSystem.cs

The progression is save in .\Optics\optics\Assets\Levels\progression.txt : the first argument is the progression of the game, the second is the number of levels

## Log
All log files are in .\Optics\optics\Assets\Logs under the form:
m d y h m s ms; time elapsed; event

Learn more about this .\Optics\optics\Assets\Components\FYFYGameEngine -> log()

## More elements
To add more types of element, create an element that extends OpticalComponent with its own propeties.

Go to .\Optics\optics\Assets\Systems\OpticalComponentSystem.cs and add this component to the _OC family.

Modify the deflect funtion of .\Optics\optics\Assets\Systems\GameEngineSystem
