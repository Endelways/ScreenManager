#### It's a small unity package, which give you ability for control windows in your application

# Documentation
#### You can use Screen as base type for create your own window type

**Example:**
```C#
    public class InventoryScreen : Screen
    {
        [SerializeField] private Transform inventoryGrid;
        [SerializeField] private InventoryMoneyView moneyView;
        private InventorySlotView[] _itemSlots;
        private Inventory _inventory;
        
        [Inject] 
        private void Setup(Inventory inventory)
        {
            _inventory = inventory;
        }
        
        public override void OnOpened(IScreenOptions options)
        {
            base.OnOpened(options);
            _itemSlots = inventoryGrid.GetComponentsInChildren<InventorySlotView>();
            foreach (var item in _inventory.GetItemList())
            {
                if (item.Item.IsNotSlotItem)
                {
                    moneyView.SetText(item.Count.ToString());
                    continue;
                }
                _itemSlots[item.SlotId].SetItem(item);
            }
        }
    }
```
In inspector you can use checkbox `IsReusableScreen` for describes YourScreen as reusable screen.

<img src="https://i.ibb.co/QbWTDvf/Screen-Manager-Unity-Package-Demo.png" alt="demo" width="280"></img>

Reusable screen can be hided without destroying and will be showed with previous data in future.
    
Now you need add this script as component in root object of window prefab.

<img src="https://i.ibb.co/mhdZZQt/Screen-Manager-Unity-Package-Demo2.png" alt="demo2" height="380"></img>

${\Large{\color{red}{\bold{\underline{Important}:}}}}$ **All screen prefabs must be in specifed folder - `Resources/Screens`**
>***Resources folder can be in any folder<br>
In Screens folder you can create any subfolder***

**Full path example - `Assets/Prefabs/Resources/Screens/InventoryScreens/ChestInventoryScreen.prefab`**

#### You can then manage this window using the service<br>
First you need create serivce, for example in di installer:<br>
`Container.BindInstance(new ScreenService(screenContainer)).AsSingle().NonLazy();`
<br>
As argument you need to use some transform which will parent for all windows.<br>
When you use `Show()` method, window prefab will be instantiated as child of this transform.<br>
You can use next methods of `ScreenService`:

+ `Show<YourScreen>(ISceenOptions options)` - Instantiates or shows previously disabled prefab with `YourScreen` component in the specifed root transform<br>
You can use `ScreenOptions` class for transfer data to `YourScreen` component<br><br>
**Example:**<br>

    In place where you executing show method from service:<br>
    ```C#
    int a = 5;
    Show<YourScreen>(new ScreenOptions<int>(a)) // - send data to YourScreen component
    ```
    In YourScreen class:<vr>
    ```C#
    public override void OnOpened(IScreenOptions options) // recive data 
    {
        base.OnOpened(options);
        if (options.Value is not int data) return; // check if data correct and cast object to your data type
        Debug.Log(data); - use data
    }
    ```

    if prefab with Component YourSceen doesn't exist, it throws exeption<br>
    if prefab was instantiated previously and now enabled nothing will happen<br>

+ `Close<YourScreen>()` - Destroys prefab with YourScreen component in the specifed root transform<br>
if prefab with Component didnt't instantiated it throws exception

+ `Hide<YourScreen>()` - Disables prefab with YourScreen component in the specifed root transform<br>
if prefab with Component didnt't instantiated it throws exception<br>
if YourScreen prefab is not ReusableScreen it will be work as `Close<YourScreen>()`

#### You can use some events in YourScreen script for recive data or control lifecycle of your objects, you need override it for use:<br>

+ `YourScreen.OnOpened(IScreenOptions options)` - executes every time when prefab with YourScreen component instatiates<br>
options is null by default<br>

+ `YourScreen.OnDisplay()` - executes every time when prefab with YourScreen component instatiates, and when hided prefabs shows again<br>

+ `YourScreen.OnClosed()` - executes every time when prefab with YourScreen component destroys<br>

+ `YourScreen.OnHide()` - executes every time when prefab with YourScreen component destroys and when reusableView hides 
