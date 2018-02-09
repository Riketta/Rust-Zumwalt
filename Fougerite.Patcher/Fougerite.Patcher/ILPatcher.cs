using System.Collections.Generic;
using System.Linq;

namespace Fougerite.Patcher
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;

    public class ILPatcher
    {
        private AssemblyDefinition rustAssembly = null;
        private AssemblyDefinition fougeriteAssembly = null;
        private TypeDefinition hooksClass = null;

        public ILPatcher()
        {
            try
            {
                rustAssembly = AssemblyDefinition.ReadAssembly("Assembly-CSharp.dll");
                // rustFirstPassAssembly = AssemblyDefinition.ReadAssembly("Assembly-CSharp-firstpass.dll");
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private void WrapWildlifeUpdateInTryCatch()
        {
            TypeDefinition type = rustAssembly.MainModule.GetType("WildlifeManager");
            TypeDefinition data = type.GetNestedType("Data");
            MethodDefinition think = data.GetMethod("Think");
            MethodDefinition update = type.GetMethod("Update");

            Instruction y = null;
            foreach (Instruction x in think.Body.Instructions)
            {
                if (x.ToString().Contains("LogException"))
                {
                    y = x;
                    break;
                }
            }
            think.Body.Instructions.Remove(y);

            TypeDefinition logger = fougeriteAssembly.MainModule.GetType("Fougerite.Logger");
            MethodDefinition logex = logger.GetMethod("LogException");

            WrapMethod(update, logex, rustAssembly, false);
            //WrapMethod(think, logex, rustAssembly, false);
        }

        private void PatchFacePunch()
        {
            AssemblyDefinition facepunch = AssemblyDefinition.ReadAssembly("Facepunch.MeshBatch.dll");
            TypeDefinition MeshBatchPhysicalOutput = facepunch.MainModule.GetType("Facepunch.MeshBatch.Runtime.MeshBatchPhysicalOutput");
            MethodDefinition ActivateImmediatelyUnchecked = MeshBatchPhysicalOutput.GetMethod("ActivateImmediatelyUnchecked");
            TypeDefinition logger = fougeriteAssembly.MainModule.GetType("Fougerite.Logger");
            MethodDefinition logex = logger.GetMethod("LogException");
            WrapMethod(ActivateImmediatelyUnchecked, logex, facepunch, false);
            facepunch.Write("Facepunch.MeshBatch.dll");
        }

        private void uLinkLateUpdateInTryCatch()
        {
            AssemblyDefinition ulink = AssemblyDefinition.ReadAssembly("uLink.dll");
            TypeDefinition type = ulink.MainModule.GetType("uLink.InternalHelper");
            TypeDefinition Class5 = ulink.MainModule.GetType("Class5");
            TypeDefinition Class56 = ulink.MainModule.GetType("Class56");
            TypeDefinition Class52 = ulink.MainModule.GetType("Class52");
            TypeDefinition Class48 = ulink.MainModule.GetType("Class48");
            TypeDefinition Struct10 = ulink.MainModule.GetType("Struct10");
            TypeDefinition Struct6 = ulink.MainModule.GetType("Struct6");
            //TypeDefinition Class46 = ulink.MainModule.GetType("Class46");
            TypeDefinition Class45 = ulink.MainModule.GetType("Class45");
            //TypeDefinition Class1 = ulink.MainModule.GetType("Class1");
            //MethodDefinition method_61 = Class1.GetMethod("method_61");
            MethodDefinition method_36 = Class56.GetMethod("method_36");
            //MethodDefinition method_44 = Class56.GetMethod("method_44");
            MethodDefinition method_20 = Class56.GetMethod("method_20");
            MethodDefinition method_25 = Class56.GetMethod("method_25");
            MethodDefinition method_22 = Class56.GetMethod("method_22");
            MethodDefinition method_435 = Class52.GetMethod("method_435");
            MethodDefinition vmethod_3 = Class52.GetMethod("vmethod_3");
            MethodDefinition method_250 = Class48.GetMethod("method_250");
            MethodDefinition method_252 = Class48.GetMethod("method_252");
            MethodDefinition method_269 = Class48.GetMethod("method_269");
            MethodDefinition method_299 = Class48.GetMethod("method_299");
            MethodDefinition method_249 = Class48.GetMethod("method_249");
            MethodDefinition method_275 = Class48.GetMethod("method_275");
            MethodDefinition method_277 = Class48.GetMethod("method_277");
            MethodDefinition method_270 = Class48.GetMethod("method_270");
            MethodDefinition method_4 = Class45.GetMethod("method_4");

            Struct6.IsPublic = true;
            Struct10.IsPublic = true;
            MethodDefinition structmethod_0 = Struct10.GetMethod("method_0");
            MethodDefinition RPCCatch = hooksClass.GetMethod("RPCCatch");
            int si = structmethod_0.Body.Instructions.Count - 46;
            ILProcessor siiLProcessor = structmethod_0.Body.GetILProcessor();
            siiLProcessor.InsertBefore(structmethod_0.Body.Instructions[si],
                Instruction.Create(OpCodes.Callvirt, ulink.MainModule.Import(RPCCatch)));
            siiLProcessor.InsertBefore(structmethod_0.Body.Instructions[si], Instruction.Create(OpCodes.Ldarg_2));

            //MethodDefinition method_124 = Class46.GetMethod("method_124");
            MethodDefinition update = type.GetMethod("LateUpdate");
            TypeDefinition logger = fougeriteAssembly.MainModule.GetType("Fougerite.Logger");

            method_277.IsPublic = true;
            method_270.IsPublic = true;
            Class5.IsPublic = true;
            Class48.IsPublic = true;
            Class56.IsPublic = true;

            MethodDefinition method = hooksClass.GetMethod("HandleuLinkDisconnect");
            MethodDefinition method2 = hooksClass.GetMethod("RPCFix");
            ILProcessor iLProcessor = method_435.Body.GetILProcessor();
            iLProcessor.Body.Instructions.Clear();
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_2));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Callvirt, ulink.MainModule.Import(method)));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));

            method_275.Body.Instructions.Clear();
            method_275.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            method_275.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
            method_275.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_2));
            method_275.Body.Instructions.Add(Instruction.Create(OpCodes.Callvirt, ulink.MainModule.Import(method2)));
            method_275.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));

            TypeDefinition Network = ulink.MainModule.GetType("uLink.Network");
            IEnumerable<MethodDefinition> CloseConnections = Network.GetMethods();
            MethodDefinition logex = logger.GetMethod("LogException");
            
            WrapMethod(method_249, logex, ulink, false);

            WrapMethod(update, logex, ulink, false);
            WrapMethod(method_36, logex, ulink, false);
            WrapMethod(method_25, logex, ulink, false);
            WrapMethod(method_22, logex, ulink, false); 

            WrapMethod(vmethod_3, logex, ulink, false);
            WrapMethod(method_250, logex, ulink, false);
            WrapMethod(method_252, logex, ulink, false);
            WrapMethod(method_269, logex, ulink, false);
            WrapMethod(method_4, logex, ulink, false);
            WrapMethod(method_299, logex, ulink, false);
            WrapMethod(method_20, logex, ulink, false);
            foreach (var x in CloseConnections.Where(x => x.Name == "CloseConnection"))
            {
                WrapMethod(x, logex, ulink, false);
            }
            /*List<Instruction> ls = method_124.Body.Instructions.Where(x => x.ToString().Contains("ArgumentOutOfRangeException") || x.ToString().Contains("throw")).ToList();
            foreach (var x in ls)
            {
                method_124.Body.Instructions.Remove(x);
            }*/
            ulink.Write("uLink.dll");
        }

        /*private void uLinkKeyDuplicationError()
        {
            AssemblyDefinition ulink = AssemblyDefinition.ReadAssembly("uLink.dll");
            TypeDefinition Class48 = ulink.MainModule.GetType("Class48");
            MethodDefinition method_299 = Class48.GetMethod("method_299");
            int Position2 = method_299.Body.Instructions.Count - 3;

             ILProcessor iLProcessor2 = method_299.Body.GetILProcessor();
             iLProcessor2.InsertBefore(method_299.Body.Instructions[Position2],
                 Instruction.Create(OpCodes.Callvirt, ulink.MainModule.Import(hooksClass.GetMethod("Ulala"))));
             iLProcessor2.InsertBefore(method_299.Body.Instructions[Position2], Instruction.Create(OpCodes.Ldarg_1));
            Instruction md = null;
            foreach (var x in method_299.Body.Instructions)
            {
                Logger.Log(" - " + x);
                if (x.ToString().Contains("Fougerite.Logger")) md = x;
            }
            Logger.Log("s: " + md);
            method_299.Body.Instructions.Remove(md);
            ulink.Write("uLink.dll");
        }*/

        private void LatePostInTryCatch()
        {
            AssemblyDefinition ulink = AssemblyDefinition.ReadAssembly("uLink.dll");
            TypeDefinition Class56 = ulink.MainModule.GetType("Class56");
            MethodDefinition method_22 = Class56.GetMethod("method_22");
            MethodDefinition method_25 = Class56.GetMethod("method_25");
            //MethodDefinition method_44 = Class56.GetMethod("method_44");
            method_22.SetPublic(true);
            method_25.SetPublic(true);
            //method_44.SetPublic(true);

            ulink.Write("uLink.dll");

            TypeDefinition type = rustAssembly.MainModule.GetType("NetCull");
            TypeDefinition type2 = type.GetNestedType("Callbacks");
            MethodDefinition def = type2.GetMethod("FirePreUpdate");
            MethodDefinition def2 = type2.GetMethod("FirePostUpdate");
            def.SetPublic(true);
            def2.SetPublic(true);
            rustAssembly.Write("Assembly-CSharp.dll");
        }

        private void LatePostInTryCatch2()
        {
            TypeDefinition type = rustAssembly.MainModule.GetType("NetCull");
            TypeDefinition type2 = type.GetNestedType("Callbacks");
            MethodDefinition def = type2.GetMethod("FirePreUpdate");
            Instruction y = null;
            foreach (Instruction x in def.Body.Instructions)
            {
                if (x.ToString().Contains("LogException"))
                {
                    y = x;
                }
            }
            def.Body.Instructions.Remove(y);

            MethodDefinition def2 = type2.GetMethod("FirePostUpdate");
            Instruction y2 = null;
            foreach (Instruction x in def.Body.Instructions)
            {
                if (x.ToString().Contains("LogException"))
                {
                    y2 = x;
                }
            }
            def2.Body.Instructions.Remove(y2);
        }
        
        private void UpdateDelegatePatch()
        {
            TypeDefinition NetCull = rustAssembly.MainModule.GetType("NetCull");
            TypeDefinition Callbacks = NetCull.GetNestedType("Callbacks");
            TypeDefinition UpdateDelegate = Callbacks.GetNestedType("UpdateDelegate");
            MethodDefinition Invoke = UpdateDelegate.GetMethod("Invoke");
            Instruction y = null;
            foreach (Instruction x in Invoke.Body.Instructions)
            {
                if (x.ToString().Contains("LogException"))
                {
                    y = x;
                }
            }
            Invoke.Body.Instructions.Remove(y);
            rustAssembly.Write("Assembly-CSharp.dll");
        }

        private void LooterPatch()
        {
            TypeDefinition type = rustAssembly.MainModule.GetType("LootableObject");
            MethodDefinition ClearLooter = type.GetMethod("ClearLooter");
            ClearLooter.SetPublic(true);
            type.GetField("_currentlyUsingPlayer").SetPublic(true);
            type.GetField("thisClientIsInWindow").SetPublic(true);
            type.GetField("occupierText").SetPublic(true);
            type.GetField("_useable").SetPublic(true);
            type.GetMethod("SendCurrentLooter").SetPublic(true);
            type.GetMethod("DestroyInExit").SetPublic(true);
            type.GetMethod("StopLooting").SetPublic(true);

            MethodDefinition SetLooter = type.GetMethod("SetLooter");
            MethodDefinition method = hooksClass.GetMethod("SetLooter");
            ILProcessor iLProcessor = SetLooter.Body.GetILProcessor();
            iLProcessor.Body.Instructions.Clear();
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Callvirt, this.rustAssembly.MainModule.Import(method)));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));

            MethodDefinition OnUseEnter = type.GetMethod("OnUseEnter");
            MethodDefinition method2 = hooksClass.GetMethod("OnUseEnter");
            ILProcessor iLProcessor2 = OnUseEnter.Body.GetILProcessor();
            iLProcessor2.Body.Instructions.Clear();
            iLProcessor2.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            iLProcessor2.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
            iLProcessor2.Body.Instructions.Add(Instruction.Create(OpCodes.Callvirt, this.rustAssembly.MainModule.Import(method2)));
            iLProcessor2.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
        }

        private void UseablePatch()
        {
            TypeDefinition Useable = rustAssembly.MainModule.GetType("Useable");
            Useable.GetMethod("Refresh").SetPublic(true);
            Useable.GetMethod("OnDestroy").SetPublic(true);
            Useable.GetMethod("Update").SetPublic(true);
            Useable.GetMethod("RunUpdate").SetPublic(true);
            Useable.GetMethod("LatchUse").SetPublic(true);
            Useable.GetMethod("Reset").SetPublic(true);
            Useable.GetField("canUse").SetPublic(true);
            Useable.GetField("_user").SetPublic(true);
            Useable.GetField("canUpdate").SetPublic(true);
            Useable.GetField("callState").SetPublic(true);
            Useable.GetNestedType("FunctionCallState").IsPublic = true;
        }

        private void ResearchPatch()
        {
            TypeDefinition type = rustAssembly.MainModule.GetType("ResearchToolItem`1");
            MethodDefinition TryCombine = type.GetMethod("TryCombine");
            MethodDefinition method = hooksClass.GetMethod("ResearchItem");
            int Position = TryCombine.Body.Instructions.Count - 13;

            ILProcessor iLProcessor = TryCombine.Body.GetILProcessor();
            iLProcessor.InsertBefore(TryCombine.Body.Instructions[Position],
                Instruction.Create(OpCodes.Callvirt, this.rustAssembly.MainModule.Import(method)));
            iLProcessor.InsertBefore(TryCombine.Body.Instructions[Position], Instruction.Create(OpCodes.Ldarg_1));
        }

        private void AntiDecay()
        {
            TypeDefinition type = rustAssembly.MainModule.GetType("EnvDecay");
            TypeDefinition definition2 = rustAssembly.MainModule.GetType("StructureMaster");

            MethodDefinition awake = type.GetMethod("Awake");
            MethodDefinition doDecay = type.GetMethod("DoDecay");
            MethodDefinition decayDisabled = hooksClass.GetMethod("DecayDisabled");

            ILProcessor iLProcessor = awake.Body.GetILProcessor();
            iLProcessor.InsertBefore(awake.Body.Instructions[0], Instruction.Create(OpCodes.Callvirt, this.rustAssembly.MainModule.Import(decayDisabled)));
            iLProcessor.InsertAfter(awake.Body.Instructions[0], Instruction.Create(OpCodes.Brtrue, awake.Body.Instructions[awake.Body.Instructions.Count - 1]));
            iLProcessor = doDecay.Body.GetILProcessor();
            iLProcessor.InsertBefore(doDecay.Body.Instructions[0], Instruction.Create(OpCodes.Callvirt, this.rustAssembly.MainModule.Import(decayDisabled)));
            iLProcessor.InsertAfter(doDecay.Body.Instructions[0], Instruction.Create(OpCodes.Brtrue, doDecay.Body.Instructions[doDecay.Body.Instructions.Count - 1]));
        }

        private void SlotOperationPatch()
        {
            TypeDefinition type = rustAssembly.MainModule.GetType("Inventory");
            IEnumerable<MethodDefinition> allmethods = type.GetMethods();
            MethodDefinition SlotOperation = null;
            foreach (MethodDefinition m in allmethods)
            {
                if (m.Name.Equals("SlotOperation") && m.Parameters.Count == 4)
                {
                    SlotOperation = m;
                    break;
                }
            }
            if (SlotOperation != null)
            {
                MethodDefinition method = hooksClass.GetMethod("FGSlotOperation");
                SlotOperation.Body.Instructions.Clear();
                SlotOperation.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                SlotOperation.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
                SlotOperation.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_2));
                SlotOperation.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_3));
                SlotOperation.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_S, SlotOperation.Parameters[3]));
                SlotOperation.Body.Instructions.Add(Instruction.Create(OpCodes.Call, this.rustAssembly.MainModule.Import(method)));
                SlotOperation.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
            }
        }

        private void RepairBenchEvent()
        {
            TypeDefinition type = rustAssembly.MainModule.GetType("RepairBench");
            MethodDefinition CompleteRepair = type.GetMethod("CompleteRepair");
            MethodDefinition method = hooksClass.GetMethod("FGCompleteRepair");
            CompleteRepair.Body.Instructions.Clear();
            CompleteRepair.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            CompleteRepair.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
            CompleteRepair.Body.Instructions.Add(Instruction.Create(OpCodes.Call, this.rustAssembly.MainModule.Import(method)));
            CompleteRepair.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
        }


        private void FieldsUpdatePatch()
        {
            TypeDefinition Metabolism = rustAssembly.MainModule.GetType("Metabolism");
            Metabolism.GetField("coreTemperature").SetPublic(true);
            Metabolism.GetField("caloricLevel").SetPublic(true);
            Metabolism.GetField("radiationLevel").SetPublic(true);
            Metabolism.GetField("waterLevelLitre").SetPublic(true);
            Metabolism.GetField("antiRads").SetPublic(true);
            Metabolism.GetField("poisonLevel").SetPublic(true);
            Metabolism.GetField("_lastWarmTime").SetPublic(true);

            TypeDefinition User = rustAssembly.MainModule.GetType("RustProto", "User");
            User.GetField("displayname_").SetPublic(true);

            TypeDefinition ResourceGivePair = rustAssembly.MainModule.GetType("ResourceGivePair");
            ResourceGivePair.GetField("_resourceItemDatablock").SetPublic(true);

            TypeDefinition StructureMaster = rustAssembly.MainModule.GetType("StructureMaster");
            StructureMaster.GetField("_structureComponents").SetPublic(true);
            StructureMaster.GetField("_weightOnMe").SetPublic(true);

            TypeDefinition InventoryHolder = rustAssembly.MainModule.GetType("InventoryHolder");
            InventoryHolder.GetField("isPlayerInventory").SetPublic(true);
            MethodDefinition m = InventoryHolder.GetMethod("GetPlayerInventory");
            m.SetPublic(true);

            TypeDefinition PlayerInventory = rustAssembly.MainModule.GetType("PlayerInventory");
            PlayerInventory.GetField("_boundBPs").SetPublic(true);

            TypeDefinition Inv2 = rustAssembly.MainModule.GetType("Inventory");
            Inv2.GetNestedType("SlotOperationsInfo").IsPublic = true;
            Inv2.GetNestedType("SlotOperations").IsPublic = true;

            TypeDefinition BasicDoor = rustAssembly.MainModule.GetType("BasicDoor");
            BasicDoor.GetField("state").SetPublic(true);
            foreach (MethodDefinition met in BasicDoor.Methods)
            {
                if (met.Name.Equals("ToggleStateServer"))
                {
                    met.SetPublic(true);
                }
            }

            TypeDefinition SleepingAvatar = rustAssembly.MainModule.GetType("SleepingAvatar");
            var methods = SleepingAvatar.Methods;
            foreach (var x in methods)
            {
                if (!x.IsPublic && x.Name == "Close")
                {
                    x.SetPublic(true);
                    break;
                }
            }

            TypeDefinition BulletWeaponDataBlock = rustAssembly.MainModule.GetType("BulletWeaponDataBlock");
            BulletWeaponDataBlock.GetMethod("ConstructItem").SetPublic(true);
            TypeDefinition ITEM_TYPE = BulletWeaponDataBlock.GetNestedType("ITEM_TYPE");
            ITEM_TYPE.IsPublic = true;
            ITEM_TYPE.IsSealed = false;
            foreach (var x in ITEM_TYPE.GetMethods())
            {
                ITEM_TYPE.GetMethod(x.Name).SetPublic(true);
            }

            TypeDefinition IBulletWeaponItem = rustAssembly.MainModule.GetType("IBulletWeaponItem");
            IBulletWeaponItem.GetProperty("cachedCasings").GetMethod.SetPublic(true);
            IBulletWeaponItem.GetProperty("cachedCasings").SetMethod.SetPublic(true);

            IBulletWeaponItem.GetProperty("clipAmmo").GetMethod.SetPublic(true);
            IBulletWeaponItem.GetProperty("clipAmmo").SetMethod.SetPublic(true);

            IBulletWeaponItem.GetProperty("clipType").GetMethod.SetPublic(true);
            //IBulletWeaponItem.GetProperty("clipType").SetMethod.SetPublic(true);

            IBulletWeaponItem.GetProperty("nextCasingsTime").GetMethod.SetPublic(true);
            //IBulletWeaponItem.GetProperty("nextCasingsTime").SetMethod.SetPublic(true);

            TypeDefinition IHeldItem = rustAssembly.MainModule.GetType("IHeldItem");

            IHeldItem.GetProperty("canActivate").GetMethod.SetPublic(true);
            //IHeldItem.GetProperty("canActivate").SetMethod.SetPublic(true);

            IHeldItem.GetProperty("canDeactivate").GetMethod.SetPublic(true);
            //IHeldItem.GetProperty("canDeactivate").SetMethod.SetPublic(true); 

            IHeldItem.GetProperty("freeModSlots").GetMethod.SetPublic(true);
            //IHeldItem.GetProperty("freeModSlots").SetMethod.SetPublic(true);

            IHeldItem.GetProperty("itemMods").GetMethod.SetPublic(true);
            //IHeldItem.GetProperty("itemMods").SetMethod.SetPublic(true);

            IHeldItem.GetProperty("itemRepresentation").GetMethod.SetPublic(true);
            IHeldItem.GetProperty("itemRepresentation").SetMethod.SetPublic(true);

            IHeldItem.GetProperty("modFlags").GetMethod.SetPublic(true);
            //IHeldItem.GetProperty("modFlags").SetMethod.SetPublic(true);

            IHeldItem.GetProperty("totalModSlots").GetMethod.SetPublic(true);
            //IHeldItem.GetProperty("totalModSlots").SetMethod.SetPublic(true);

            IHeldItem.GetProperty("usedModSlots").GetMethod.SetPublic(true);
            //IHeldItem.GetProperty("usedModSlots").SetMethod.SetPublic(true);

            IHeldItem.GetProperty("viewModelInstance").GetMethod.SetPublic(true);
            //IHeldItem.GetProperty("viewModelInstance").SetMethod.SetPublic(true);

            IHeldItem.GetMethod("AddMod").SetPublic(true);
            IHeldItem.GetMethod("FindMod").SetPublic(true);
            IHeldItem.GetMethod("ServerFrame").SetPublic(true);
            IHeldItem.GetMethod("SetTotalModSlotCount").SetPublic(true);
            IHeldItem.GetMethod("SetUsedModSlotCount").SetPublic(true);

            TypeDefinition IInventoryItem = rustAssembly.MainModule.GetType("IInventoryItem");

            IInventoryItem.GetProperty("active").GetMethod.SetPublic(true);
            //IInventoryItem.GetProperty("active").SetMethod.SetPublic(true);

            IInventoryItem.GetProperty("character").GetMethod.SetPublic(true);
            //IInventoryItem.GetProperty("character").SetMethod.SetPublic(true);

            IInventoryItem.GetProperty("condition").GetMethod.SetPublic(true);
            //IInventoryItem.GetProperty("condition").SetMethod.SetPublic(true);

            IInventoryItem.GetProperty("controllable").GetMethod.SetPublic(true);
            //IInventoryItem.GetProperty("controllable").SetMethod.SetPublic(true);

            IInventoryItem.GetProperty("controller").GetMethod.SetPublic(true);
            //IInventoryItem.GetProperty("controller").SetMethod.SetPublic(true);

            IInventoryItem.GetProperty("datablock").GetMethod.SetPublic(true);
            //IInventoryItem.GetProperty("datablock").SetMethod.SetPublic(true);

            IInventoryItem.GetProperty("dirty").GetMethod.SetPublic(true);
            //IInventoryItem.GetProperty("dirty").SetMethod.SetPublic(true);

            IInventoryItem.GetProperty("doNotSave").GetMethod.SetPublic(true);
            //IInventoryItem.GetProperty("doNotSave").SetMethod.SetPublic(true);

            IInventoryItem.GetProperty("idMain").GetMethod.SetPublic(true);
            //IInventoryItem.GetProperty("idMain").SetMethod.SetPublic(true);

            IInventoryItem.GetProperty("inventory").GetMethod.SetPublic(true);
            //IInventoryItem.GetProperty("inventory").SetMethod.SetPublic(true);

            IInventoryItem.GetProperty("isInLocalInventory").GetMethod.SetPublic(true);
            //IInventoryItem.GetProperty("isInLocalInventory").SetMethod.SetPublic(true);

            IInventoryItem.GetProperty("lastUseTime").GetMethod.SetPublic(true);
            IInventoryItem.GetProperty("lastUseTime").SetMethod.SetPublic(true);

            IInventoryItem.GetProperty("maxcondition").GetMethod.SetPublic(true);
            //IInventoryItem.GetProperty("maxcondition").SetMethod.SetPublic(true);

            IInventoryItem.GetProperty("slot").GetMethod.SetPublic(true);
            //IInventoryItem.GetProperty("slot").SetMethod.SetPublic(true);

            IInventoryItem.GetProperty("toolTip").GetMethod.SetPublic(true);
            //IInventoryItem.GetProperty("toolTip").SetMethod.SetPublic(true);

            IInventoryItem.GetProperty("uses").GetMethod.SetPublic(true);
            //IInventoryItem.GetProperty("uses").SetMethod.SetPublic(true);

            IInventoryItem.GetMethod("AddUses").SetPublic(true);
            IInventoryItem.GetMethod("Consume").SetPublic(true);
            IInventoryItem.GetMethod("FireClientSideItemEvent").SetPublic(true);
            IInventoryItem.GetMethod("GetConditionPercent").SetPublic(true);
            IInventoryItem.GetMethod("IsBroken").SetPublic(true);
            IInventoryItem.GetMethod("IsDamaged").SetPublic(true);
            IInventoryItem.GetMethod("Load").SetPublic(true);
            IInventoryItem.GetMethod("MarkDirty").SetPublic(true);
            IInventoryItem.GetMethod("OnAddedTo").SetPublic(true);
            IInventoryItem.GetMethod("OnBeltUse").SetPublic(true);
            IInventoryItem.GetMethod("OnMenuOption").SetPublic(true);
            IInventoryItem.GetMethod("OnMovedTo").SetPublic(true);
            IInventoryItem.GetMethod("Save").SetPublic(true);
            IInventoryItem.GetMethod("SetCondition").SetPublic(true);
            IInventoryItem.GetMethod("SetMaxCondition").SetPublic(true);
            IInventoryItem.GetMethod("SetUses").SetPublic(true);
            IInventoryItem.GetMethod("TryCombine").SetPublic(true);
            IInventoryItem.GetMethod("TryConditionLoss").SetPublic(true);
            IInventoryItem.GetMethod("TryStack").SetPublic(true);

            TypeDefinition IWeaponItem = rustAssembly.MainModule.GetType("IWeaponItem");

            IWeaponItem.GetProperty("canAim").GetMethod.SetPublic(true);
            //IWeaponItem.GetProperty("canAim").SetMethod.SetPublic(true);

            IWeaponItem.GetProperty("canPrimaryAttack").GetMethod.SetPublic(true);
            //IWeaponItem.GetProperty("canPrimaryAttack").SetMethod.SetPublic(true);

            IWeaponItem.GetProperty("canSecondaryAttack").GetMethod.SetPublic(true);
            //IWeaponItem.GetProperty("canSecondaryAttack").SetMethod.SetPublic(true);

            IWeaponItem.GetProperty("deployed").GetMethod.SetPublic(true);
            //IWeaponItem.GetProperty("deployed").SetMethod.SetPublic(true);

            IWeaponItem.GetProperty("deployFinishedTime").GetMethod.SetPublic(true);
            IWeaponItem.GetProperty("deployFinishedTime").SetMethod.SetPublic(true);

            IWeaponItem.GetProperty("nextPrimaryAttackTime").GetMethod.SetPublic(true);
            IWeaponItem.GetProperty("nextPrimaryAttackTime").SetMethod.SetPublic(true);

            IWeaponItem.GetProperty("nextSecondaryAttackTime").GetMethod.SetPublic(true);
            IWeaponItem.GetProperty("nextSecondaryAttackTime").SetMethod.SetPublic(true);

            IWeaponItem.GetProperty("possibleReloadCount").GetMethod.SetPublic(true);
            //IWeaponItem.GetProperty("possibleReloadCount").SetMethod.SetPublic(true);

            IWeaponItem.GetMethod("PrimaryAttack").SetPublic(true);
            IWeaponItem.GetMethod("Reload").SetPublic(true);
            IWeaponItem.GetMethod("SecondaryAttack").SetPublic(true);
            IWeaponItem.GetMethod("ValidatePrimaryMessageTime").SetPublic(true);


            ITEM_TYPE.GetMethod("IBulletWeaponItem.get_cachedCasings").SetPublic(true);
            ITEM_TYPE.GetMethod("IBulletWeaponItem.get_clipAmmo").SetPublic(true);
            ITEM_TYPE.GetMethod("IBulletWeaponItem.get_clipType").SetPublic(true);
            ITEM_TYPE.GetMethod("IBulletWeaponItem.get_nextCasingsTime").SetPublic(true);
            ITEM_TYPE.GetMethod("IBulletWeaponItem.set_cachedCasings").SetPublic(true);
            ITEM_TYPE.GetMethod("IBulletWeaponItem.set_clipAmmo").SetPublic(true);
            ITEM_TYPE.GetMethod("IBulletWeaponItem.set_nextCasingsTime").SetPublic(true);
            ITEM_TYPE.GetMethod("IHeldItem.AddMod").SetPublic(true);
            ITEM_TYPE.GetMethod("IHeldItem.FindMod").SetPublic(true);
            ITEM_TYPE.GetMethod("IHeldItem.get_canActivate").SetPublic(true);
            ITEM_TYPE.GetMethod("IHeldItem.get_canDeactivate").SetPublic(true);
            ITEM_TYPE.GetMethod("IHeldItem.get_freeModSlots").SetPublic(true);
            ITEM_TYPE.GetMethod("IHeldItem.get_itemMods").SetPublic(true);
            ITEM_TYPE.GetMethod("IHeldItem.get_itemRepresentation").SetPublic(true);
            ITEM_TYPE.GetMethod("IHeldItem.get_modFlags").SetPublic(true);
            ITEM_TYPE.GetMethod("IHeldItem.get_totalModSlots").SetPublic(true);
            ITEM_TYPE.GetMethod("IHeldItem.get_usedModSlots").SetPublic(true);
            ITEM_TYPE.GetMethod("IHeldItem.get_viewModelInstance").SetPublic(true);
            ITEM_TYPE.GetMethod("IHeldItem.set_itemRepresentation").SetPublic(true);
            ITEM_TYPE.GetMethod("IHeldItem.SetTotalModSlotCount").SetPublic(true);
            ITEM_TYPE.GetMethod("IHeldItem.SetUsedModSlotCount").SetPublic(true);
            ITEM_TYPE.GetMethod("IInventoryItem.AddUses").SetPublic(true);
            ITEM_TYPE.GetMethod("IInventoryItem.Consume").SetPublic(true);
            ITEM_TYPE.GetMethod("IInventoryItem.get_active").SetPublic(true);
            ITEM_TYPE.GetMethod("IInventoryItem.get_character").SetPublic(true);
            ITEM_TYPE.GetMethod("IInventoryItem.get_condition").SetPublic(true);
            ITEM_TYPE.GetMethod("IInventoryItem.get_controllable").SetPublic(true);
            ITEM_TYPE.GetMethod("IInventoryItem.get_controller").SetPublic(true);
            ITEM_TYPE.GetMethod("IInventoryItem.get_dirty").SetPublic(true);
            ITEM_TYPE.GetMethod("IInventoryItem.get_idMain").SetPublic(true);
            ITEM_TYPE.GetMethod("IInventoryItem.get_inventory").SetPublic(true);
            ITEM_TYPE.GetMethod("IInventoryItem.get_isInLocalInventory").SetPublic(true);
            ITEM_TYPE.GetMethod("IInventoryItem.get_lastUseTime").SetPublic(true);
            ITEM_TYPE.GetMethod("IInventoryItem.get_maxcondition").SetPublic(true);
            ITEM_TYPE.GetMethod("IInventoryItem.get_slot").SetPublic(true);
            ITEM_TYPE.GetMethod("IInventoryItem.get_uses").SetPublic(true);
            ITEM_TYPE.GetMethod("IInventoryItem.GetConditionPercent").SetPublic(true);
            ITEM_TYPE.GetMethod("IInventoryItem.IsBroken").SetPublic(true);
            ITEM_TYPE.GetMethod("IInventoryItem.IsDamaged").SetPublic(true);
            ITEM_TYPE.GetMethod("IInventoryItem.Load").SetPublic(true);
            ITEM_TYPE.GetMethod("IInventoryItem.MarkDirty").SetPublic(true);
            ITEM_TYPE.GetMethod("IInventoryItem.Save").SetPublic(true);
            ITEM_TYPE.GetMethod("IInventoryItem.set_lastUseTime").SetPublic(true);
            ITEM_TYPE.GetMethod("IInventoryItem.SetCondition").SetPublic(true);
            ITEM_TYPE.GetMethod("IInventoryItem.SetMaxCondition").SetPublic(true);
            ITEM_TYPE.GetMethod("IInventoryItem.SetUses").SetPublic(true);
            ITEM_TYPE.GetMethod("IInventoryItem.TryConditionLoss").SetPublic(true);
            ITEM_TYPE.GetMethod("IWeaponItem.get_canAim").SetPublic(true);
            ITEM_TYPE.GetMethod("IWeaponItem.get_deployFinishedTime").SetPublic(true);
            ITEM_TYPE.GetMethod("IWeaponItem.get_nextPrimaryAttackTime").SetPublic(true);
            ITEM_TYPE.GetMethod("IWeaponItem.get_nextSecondaryAttackTime").SetPublic(true);
            ITEM_TYPE.GetMethod("IWeaponItem.set_deployFinishedTime").SetPublic(true);
            ITEM_TYPE.GetMethod("IWeaponItem.set_nextPrimaryAttackTime").SetPublic(true);
            ITEM_TYPE.GetMethod("IWeaponItem.set_nextSecondaryAttackTime").SetPublic(true);
            ITEM_TYPE.GetMethod("IWeaponItem.ValidatePrimaryMessageTime").SetPublic(true);
            ITEM_TYPE.GetProperty("IInventoryItem.datablock").GetMethod.SetPublic(true);

            TypeDefinition ItemModDataBlock = rustAssembly.MainModule.GetType("ItemModDataBlock");
            ItemModDataBlock.GetMethod("ConstructItem").SetPublic(true);
            ITEM_TYPE = ItemModDataBlock.GetNestedType("ITEM_TYPE");
            ITEM_TYPE.IsPublic = true;
            ITEM_TYPE.IsSealed = false;
            TypeDefinition ItemDataBlock = rustAssembly.MainModule.GetType("ItemDataBlock");
            ItemDataBlock.GetMethod("ConstructItem").SetPublic(true);
            ITEM_TYPE = ItemDataBlock.GetNestedType("ITEM_TYPE");
            ITEM_TYPE.IsPublic = true;
            ITEM_TYPE.IsSealed = false;
            //TypeDefinition ServerManagement = rustAssembly.MainModule.GetType("ServerManagement");
            //MethodDefinition EraseCharactersForClient = ServerManagement.GetMethod("EraseCharactersForClient");
            //EraseCharactersForClient.SetPublic(true);

            /*TypeDefinition Inventory = rustAssembly.MainModule.GetType("Inventory");
            TypeDefinition SlotOperationsInfo = Inventory.GetNestedType("SlotOperationsInfo").;
            Logger.Log(SlotOperationsInfo.ToString());*/

            /*TypeDefinition wildlifeManager = rustAssembly.MainModule.GetType("WildlifeManager");
            wildlifeManager.GetNestedType("Data").SetPublic(true);*/
        }

        private void MetaBolismMethodMod()
        {
            TypeDefinition Metabolism = rustAssembly.MainModule.GetType("Metabolism");
            MethodDefinition method = hooksClass.GetMethod("RecieveNetwork");
            MethodDefinition RecieveNetwork = Metabolism.GetMethod("RecieveNetwork");
            this.CloneMethod(RecieveNetwork);
            Array a = RecieveNetwork.Parameters.ToArray();
            RecieveNetwork.Body.Instructions.Clear();
            RecieveNetwork.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            foreach (ParameterDefinition p in a)
            {
                RecieveNetwork.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_S, p));
            }
            RecieveNetwork.Body.Instructions.Add(Instruction.Create(OpCodes.Callvirt, this.rustAssembly.MainModule.Import(method)));
            RecieveNetwork.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));

            TypeDefinition logger = fougeriteAssembly.MainModule.GetType("Fougerite.Logger");
            MethodDefinition logex = logger.GetMethod("LogException");

            WrapMethod(RecieveNetwork, logex, rustAssembly, false);
        }

        private void ItemPickupHook()
        {
            TypeDefinition ItemPickup = rustAssembly.MainModule.GetType("ItemPickup");
            MethodDefinition PlayerUse = ItemPickup.GetMethod("PlayerUse");
            MethodDefinition method = hooksClass.GetMethod("ItemPickup");

            ILProcessor iLProcessor = PlayerUse.Body.GetILProcessor();
            int Position = PlayerUse.Body.Instructions.Count - 26;
            iLProcessor.InsertBefore(PlayerUse.Body.Instructions[Position],
                Instruction.Create(OpCodes.Callvirt, this.rustAssembly.MainModule.Import(method)));
            iLProcessor.InsertBefore(PlayerUse.Body.Instructions[Position], Instruction.Create(OpCodes.Ldloc_3));
            iLProcessor.InsertBefore(PlayerUse.Body.Instructions[Position], Instruction.Create(OpCodes.Ldloc_1));
            iLProcessor.InsertBefore(PlayerUse.Body.Instructions[Position], Instruction.Create(OpCodes.Ldloc_2));
            iLProcessor.InsertBefore(PlayerUse.Body.Instructions[Position], Instruction.Create(OpCodes.Ldarg_1));
        }

        private void FallDamageHook()
        {
            TypeDefinition FallDamage = rustAssembly.MainModule.GetType("FallDamage");
            MethodDefinition FallImpact = FallDamage.GetMethod("FallImpact");
            MethodDefinition method = hooksClass.GetMethod("FallDamage");

            ILProcessor iLProcessor = FallImpact.Body.GetILProcessor();
            int Position = FallImpact.Body.Instructions.Count - 1;
            iLProcessor.InsertBefore(FallImpact.Body.Instructions[Position],
                Instruction.Create(OpCodes.Callvirt, this.rustAssembly.MainModule.Import(method)));
            iLProcessor.InsertBefore(FallImpact.Body.Instructions[Position], Instruction.Create(OpCodes.Ldloc_2));
            iLProcessor.InsertBefore(FallImpact.Body.Instructions[Position], Instruction.Create(OpCodes.Ldloc_1));
            iLProcessor.InsertBefore(FallImpact.Body.Instructions[Position], Instruction.Create(OpCodes.Ldloc_0));
            iLProcessor.InsertBefore(FallImpact.Body.Instructions[Position], Instruction.Create(OpCodes.Ldarg_1));
            iLProcessor.InsertBefore(FallImpact.Body.Instructions[Position], Instruction.Create(OpCodes.Ldarg_0));

            MethodDefinition FallDamageCheck = hooksClass.GetMethod("FallDamageCheck");

            MethodDefinition flo = FallDamage.GetMethod("fIo");
            ILProcessor iLProcessor2 = flo.Body.GetILProcessor();
            iLProcessor2.Body.Instructions.Clear();
            iLProcessor2.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            iLProcessor2.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
            iLProcessor2.Body.Instructions.Add(Instruction.Create(OpCodes.Call, rustAssembly.MainModule.Import(FallDamageCheck)));
            iLProcessor2.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));

        }

        private void HumanControllerPatch()
        {
            TypeDefinition HumanController = rustAssembly.MainModule.GetType("HumanController");
            MethodDefinition method = hooksClass.GetMethod("ClientMove");
            MethodDefinition GetClientMove = HumanController.GetMethod("GetClientMove");
            this.CloneMethod(GetClientMove);
            Array a = GetClientMove.Parameters.ToArray();
            Array.Reverse(a);
            ILProcessor iLProcessor = GetClientMove.Body.GetILProcessor();
            iLProcessor.InsertBefore(GetClientMove.Body.Instructions[0],
                Instruction.Create(OpCodes.Callvirt, this.rustAssembly.MainModule.Import(method)));
            foreach (ParameterDefinition p in a)
            {
                iLProcessor.InsertBefore(GetClientMove.Body.Instructions[0], Instruction.Create(OpCodes.Ldarg_S, p));
            }
            iLProcessor.InsertBefore(GetClientMove.Body.Instructions[0], Instruction.Create(OpCodes.Ldarg_0));

            TypeDefinition logger = fougeriteAssembly.MainModule.GetType("Fougerite.Logger");
            MethodDefinition logex = logger.GetMethod("LogException");
            WrapMethod(GetClientMove, logex, rustAssembly, false);
        }

        private void CraftingPatch()
        {
            TypeDefinition CraftingInventory = rustAssembly.MainModule.GetType("CraftingInventory");
            MethodDefinition CancelCrafting = CraftingInventory.GetMethod("CancelCrafting");
            CancelCrafting.SetPublic(true);
            MethodDefinition StartCrafting = null;
            IEnumerable<MethodDefinition> allmethods = CraftingInventory.GetMethods();
            foreach (MethodDefinition m in allmethods)
            {
                if (m.Name.Equals("StartCrafting") && m.Parameters.Count == 3)
                {
                    StartCrafting = m;
                    break;
                }
            }
            MethodDefinition method = hooksClass.GetMethod("CraftingEvent");

            this.CloneMethod(StartCrafting);

            ILProcessor iLProcessor = StartCrafting.Body.GetILProcessor();
            int Position = StartCrafting.Body.Instructions.Count - 2;
            iLProcessor.InsertBefore(StartCrafting.Body.Instructions[Position], 
                Instruction.Create(OpCodes.Callvirt, this.rustAssembly.MainModule.Import(method)));
            iLProcessor.InsertBefore(StartCrafting.Body.Instructions[Position], Instruction.Create(OpCodes.Ldarg_3));
            iLProcessor.InsertBefore(StartCrafting.Body.Instructions[Position], Instruction.Create(OpCodes.Ldarg_2));
            iLProcessor.InsertBefore(StartCrafting.Body.Instructions[Position], Instruction.Create(OpCodes.Ldarg_1));
            iLProcessor.InsertBefore(StartCrafting.Body.Instructions[Position], Instruction.Create(OpCodes.Ldarg_0));
        }

        private void NavMeshPatch()
        {
            TypeDefinition BasicWildLifeMovement = rustAssembly.MainModule.GetType("BaseAIMovement");
            MethodDefinition DoMove = BasicWildLifeMovement.GetMethod("DoMove");
            MethodDefinition method = hooksClass.GetMethod("AnimalMovement");
            this.CloneMethod(DoMove);

            ILProcessor iLProcessor = DoMove.Body.GetILProcessor();
            iLProcessor.InsertBefore(DoMove.Body.Instructions[0],
                Instruction.Create(OpCodes.Callvirt, this.rustAssembly.MainModule.Import(method)));
            iLProcessor.InsertBefore(DoMove.Body.Instructions[0], Instruction.Create(OpCodes.Ldarg_2));
            iLProcessor.InsertBefore(DoMove.Body.Instructions[0], Instruction.Create(OpCodes.Ldarg_1));
            iLProcessor.InsertBefore(DoMove.Body.Instructions[0], Instruction.Create(OpCodes.Ldarg_0));

        }

        private void ResourceSpawned()
        {
            TypeDefinition ResourceTarget = rustAssembly.MainModule.GetType("ResourceTarget");
            ResourceTarget.GetField("gatherProgress").SetPublic(true);
            ResourceTarget.GetField("startingTotal").SetPublic(true);

            MethodDefinition Awake = ResourceTarget.GetMethod("Awake");
            this.CloneMethod(Awake);

            MethodDefinition method = hooksClass.GetMethod("ResourceSpawned");
            ILProcessor iLProcessor = Awake.Body.GetILProcessor();

            int c = Awake.Body.Instructions.Count - 1;

            iLProcessor.InsertBefore(Awake.Body.Instructions[c],
                Instruction.Create(OpCodes.Callvirt, this.rustAssembly.MainModule.Import(method)));
            iLProcessor.InsertBefore(Awake.Body.Instructions[c], Instruction.Create(OpCodes.Ldarg_0));
        }

        private void InventoryModifications()
        {
            TypeDefinition Inventory = rustAssembly.MainModule.GetType("Inventory");
            Inventory.GetField("_netListeners").SetPublic(true);
            MethodDefinition ItemRemoved = Inventory.GetMethod("ItemRemoved");
            this.CloneMethod(ItemRemoved);

            MethodDefinition method = hooksClass.GetMethod("ItemRemoved");
            ILProcessor iLProcessor = ItemRemoved.Body.GetILProcessor();

            iLProcessor.InsertBefore(ItemRemoved.Body.Instructions[0],
                Instruction.Create(OpCodes.Callvirt, this.rustAssembly.MainModule.Import(method)));
            iLProcessor.InsertBefore(ItemRemoved.Body.Instructions[0], Instruction.Create(OpCodes.Ldarg_2));
            iLProcessor.InsertBefore(ItemRemoved.Body.Instructions[0], Instruction.Create(OpCodes.Ldarg_1));
            iLProcessor.InsertBefore(ItemRemoved.Body.Instructions[0], Instruction.Create(OpCodes.Ldarg_0));
            TypeDefinition logger = fougeriteAssembly.MainModule.GetType("Fougerite.Logger");
            MethodDefinition logex = logger.GetMethod("LogException");
            WrapMethod(ItemRemoved, logex, rustAssembly, false);
        }

        private void InventoryModifications2()
        {
            TypeDefinition Inventory = rustAssembly.MainModule.GetType("Inventory");
            MethodDefinition ItemAdded = Inventory.GetMethod("ItemAdded");
            ILProcessor iLProcessor = ItemAdded.Body.GetILProcessor();
            this.CloneMethod(ItemAdded);

            MethodDefinition method2 = hooksClass.GetMethod("ItemAdded");

            iLProcessor.InsertBefore(ItemAdded.Body.Instructions[0],
                Instruction.Create(OpCodes.Callvirt, this.rustAssembly.MainModule.Import(method2)));
            iLProcessor.InsertBefore(ItemAdded.Body.Instructions[0], Instruction.Create(OpCodes.Ldarg_2));
            iLProcessor.InsertBefore(ItemAdded.Body.Instructions[0], Instruction.Create(OpCodes.Ldarg_1));
            iLProcessor.InsertBefore(ItemAdded.Body.Instructions[0], Instruction.Create(OpCodes.Ldarg_0));
            TypeDefinition logger = fougeriteAssembly.MainModule.GetType("Fougerite.Logger");
            MethodDefinition logex = logger.GetMethod("LogException");
            WrapMethod(ItemAdded, logex, rustAssembly, false);
        }

        private void AirdropPatch()
        {
            TypeDefinition SupplyDropZone = rustAssembly.MainModule.GetType("SupplyDropZone");
            MethodDefinition GetRandomTargetPos = SupplyDropZone.GetMethod("GetRandomTargetPos");
            this.CloneMethod(GetRandomTargetPos);

            int Position = GetRandomTargetPos.Body.Instructions.Count - 1;

            MethodDefinition method = hooksClass.GetMethod("Airdrop2");
            ILProcessor iLProcessor = GetRandomTargetPos.Body.GetILProcessor();
            iLProcessor.InsertBefore(GetRandomTargetPos.Body.Instructions[Position],
                Instruction.Create(OpCodes.Callvirt, this.rustAssembly.MainModule.Import(method)));
            iLProcessor.InsertBefore(GetRandomTargetPos.Body.Instructions[Position], Instruction.Create(OpCodes.Ldloc_0));


            /*TypeDefinition SupplyDropPlane = rustAssembly.MainModule.GetType("SupplyDropPlane");
            MethodDefinition DropCrate = SupplyDropPlane.GetMethod("DropCrate");
            this.CloneMethod(DropCrate);
            Position = DropCrate.Body.Instructions.Count - 1;
            method = hooksClass.GetMethod("AirdropCrateDropped");
            iLProcessor = DropCrate.Body.GetILProcessor();
            iLProcessor.InsertBefore(DropCrate.Body.Instructions[Position],
                Instruction.Create(OpCodes.Callvirt, this.rustAssembly.MainModule.Import(method)));
            iLProcessor.InsertBefore(DropCrate.Body.Instructions[Position], Instruction.Create(OpCodes.Ldloc_1));*/
        }

        private void ClientConnectionPatch()
        {
            TypeDefinition ClientConnection = rustAssembly.MainModule.GetType("ClientConnection");
            MethodDefinition DenyAccess = ClientConnection.GetMethod("DenyAccess");
            ILProcessor iLProcessor = DenyAccess.Body.GetILProcessor();
            this.CloneMethod(DenyAccess);
            MethodDefinition method = hooksClass.GetMethod("SteamDeny");
            iLProcessor.Body.Instructions.Clear();
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_2));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_3));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Callvirt, this.rustAssembly.MainModule.Import(method)));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
        }

        private void ConnectionAcceptorPatch()
        {
            TypeDefinition ConnectionAcceptor = rustAssembly.MainModule.GetType("ConnectionAcceptor");
            MethodDefinition uLink_OnPlayerApproval = ConnectionAcceptor.GetMethod("uLink_OnPlayerApproval");
            ILProcessor iLProcessor = uLink_OnPlayerApproval.Body.GetILProcessor();
            this.CloneMethod(uLink_OnPlayerApproval);
            MethodDefinition method = hooksClass.GetMethod("PlayerApproval");
            iLProcessor.Body.Instructions.Clear();
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Callvirt, this.rustAssembly.MainModule.Import(method)));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
        }

        private void SupplyCratePatch()
        {
            TypeDefinition SupplyCrate = rustAssembly.MainModule.GetType("SupplyCrate");
            MethodDefinition FixedUpdate = SupplyCrate.GetMethod("FixedUpdate");
            MethodDefinition DoNetwork = SupplyCrate.GetMethod("DoNetwork");

            TypeDefinition logger = fougeriteAssembly.MainModule.GetType("Fougerite.Logger");
            MethodDefinition logex = logger.GetMethod("LogException");
            WrapMethod(FixedUpdate, logex, rustAssembly, false);
            WrapMethod(DoNetwork, logex, rustAssembly, false);
        }

        private void BootstrapAttachPatch()
        {
            TypeDefinition fougeriteBootstrap = fougeriteAssembly.MainModule.GetType("Fougerite.Bootstrap");
            TypeDefinition serverInit = rustAssembly.MainModule.GetType("ServerInit");
            MethodDefinition attachBootstrap = fougeriteBootstrap.GetMethod("AttachBootstrap");
            MethodDefinition awake = serverInit.GetMethod("Awake");
            awake.Body.GetILProcessor().InsertAfter(awake.Body.Instructions[0x74], Instruction.Create(OpCodes.Call, this.rustAssembly.MainModule.Import(attachBootstrap)));
        }

        private void EntityDecayPatch_StructureMaster()
        {
            TypeDefinition type = rustAssembly.MainModule.GetType("StructureMaster");
            MethodDefinition orig = type.GetMethod("DoDecay");
            MethodDefinition method = hooksClass.GetMethod("EntityDecay");

            this.CloneMethod(orig);
            ILProcessor iLProcessor = orig.Body.GetILProcessor();
            iLProcessor.InsertBefore(orig.Body.Instructions[244], Instruction.Create(OpCodes.Stloc_S, orig.Body.Variables[6]));
            iLProcessor.InsertBefore(orig.Body.Instructions[244], Instruction.Create(OpCodes.Callvirt, this.rustAssembly.MainModule.Import(method)));
            iLProcessor.InsertBefore(orig.Body.Instructions[244], Instruction.Create(OpCodes.Ldloc_S, orig.Body.Variables[6]));
            iLProcessor.InsertBefore(orig.Body.Instructions[244], Instruction.Create(OpCodes.Ldloc_3));
        }

        private void EntityDecayPatch_EnvDecay()
        {
            TypeDefinition type = rustAssembly.MainModule.GetType("EnvDecay");
            MethodDefinition orig = type.GetMethod("DecayThink");
            MethodDefinition method = hooksClass.GetMethod("EntityDecay");
            FieldDefinition Field = type.GetField("_deployable");

            this.CloneMethod(orig);
            ILProcessor iLProcessor = orig.Body.GetILProcessor();
            iLProcessor.InsertBefore(orig.Body.Instructions[49], Instruction.Create(OpCodes.Stloc_S, orig.Body.Variables[2]));
            iLProcessor.InsertBefore(orig.Body.Instructions[49], Instruction.Create(OpCodes.Callvirt, this.rustAssembly.MainModule.Import(method)));
            iLProcessor.InsertBefore(orig.Body.Instructions[49], Instruction.Create(OpCodes.Ldloc_S, orig.Body.Variables[2]));
            iLProcessor.InsertBefore(orig.Body.Instructions[49], Instruction.Create(OpCodes.Ldfld, Field));
            iLProcessor.InsertBefore(orig.Body.Instructions[49], Instruction.Create(OpCodes.Ldarg_0));
        }

        private void NPCHurtKilledPatch_BasicWildLifeAI()
        {
            TypeDefinition type = rustAssembly.MainModule.GetType("BasicWildLifeAI");
            MethodDefinition orig = type.GetMethod("OnHurt");
            MethodDefinition method = hooksClass.GetMethod("NPCHurt");

            MethodDefinition NPCKilled = type.GetMethod("OnKilled");
            MethodDefinition NPCKilledHook = hooksClass.GetMethod("NPCKilled");

            this.CloneMethod(orig);
            // OldNPC Hurt
            //ILProcessor iLProcessor = orig.Body.GetILProcessor();
            //iLProcessor.InsertBefore(orig.Body.Instructions[0], Instruction.Create(OpCodes.Call, this.rustAssembly.MainModule.Import(method)));
            //iLProcessor.InsertBefore(orig.Body.Instructions[0], Instruction.Create(OpCodes.Ldarga_S, orig.Parameters[0]));

            // OldNPC Killed
            //iLProcessor = NPCKilled.Body.GetILProcessor();
            //iLProcessor.InsertBefore(NPCKilled.Body.Instructions[0], Instruction.Create(OpCodes.Call, this.rustAssembly.MainModule.Import(NPCKilledHook)));
            //iLProcessor.InsertBefore(NPCKilled.Body.Instructions[0], Instruction.Create(OpCodes.Ldarga_S, NPCKilled.Parameters[0]));
        }

        private void NPCHurtPatch_HostileWildlifeAI()
        {
            TypeDefinition type = rustAssembly.MainModule.GetType("HostileWildlifeAI");
            MethodDefinition orig = type.GetMethod("OnHurt");
            MethodDefinition method = hooksClass.GetMethod("NPCHurt");

            this.CloneMethod(orig);
            ILProcessor iLProcessor = orig.Body.GetILProcessor();
            iLProcessor.InsertBefore(orig.Body.Instructions[0], Instruction.Create(OpCodes.Call, this.rustAssembly.MainModule.Import(method)));
            iLProcessor.InsertBefore(orig.Body.Instructions[0], Instruction.Create(OpCodes.Ldarga_S, orig.Parameters[0]));
        }

        private void PlayerSpawningSpawnedPatch()
        {
            TypeDefinition type = rustAssembly.MainModule.GetType("ServerManagement");
            MethodDefinition orig = type.GetMethod("SpawnPlayer");
            MethodDefinition method = hooksClass.GetMethod("PlayerSpawning");
            MethodDefinition SpawnedHook = hooksClass.GetMethod("PlayerSpawned");

            this.CloneMethod(orig);
            ILProcessor iLProcessor = orig.Body.GetILProcessor();

            // 141 - playerFor.hasLastKnownPosition = true;
            int Position = orig.Body.Instructions.Count - 2;
            iLProcessor.InsertBefore(orig.Body.Instructions[Position], Instruction.Create(OpCodes.Call, this.rustAssembly.MainModule.Import(SpawnedHook)));
            iLProcessor.InsertBefore(orig.Body.Instructions[Position], Instruction.Create(OpCodes.Ldarg_2));
            iLProcessor.InsertBefore(orig.Body.Instructions[Position], Instruction.Create(OpCodes.Ldloc_0));
            iLProcessor.InsertBefore(orig.Body.Instructions[Position], Instruction.Create(OpCodes.Ldarg_1));

            // 114 - user.truthDetector.NoteTeleported(zero, 0.0);
            iLProcessor.InsertBefore(orig.Body.Instructions[114], Instruction.Create(OpCodes.Stloc_0));
            iLProcessor.InsertBefore(orig.Body.Instructions[114], Instruction.Create(OpCodes.Call, this.rustAssembly.MainModule.Import(method)));
            iLProcessor.InsertBefore(orig.Body.Instructions[114], Instruction.Create(OpCodes.Ldarg_2));
            iLProcessor.InsertBefore(orig.Body.Instructions[114], Instruction.Create(OpCodes.Ldloc_0));
            iLProcessor.InsertBefore(orig.Body.Instructions[114], Instruction.Create(OpCodes.Ldarg_1));
        }

        private void ServerShutdownPatch()
        {
            TypeDefinition type = rustAssembly.MainModule.GetType("LibRust");
            MethodDefinition orig = type.GetMethod("OnDestroy");
            MethodDefinition method = hooksClass.GetMethod("ServerShutdown");

            this.CloneMethod(orig);
            ILProcessor iLProcessor = orig.Body.GetILProcessor(); // 5 - Shutdown();
            iLProcessor.InsertBefore(orig.Body.Instructions[5], Instruction.Create(OpCodes.Call, this.rustAssembly.MainModule.Import(method)));
        }

        private void PlayerGatherWoodPatch()
        {
            TypeDefinition type = rustAssembly.MainModule.GetType("MeleeWeaponDataBlock");
            MethodDefinition orig = type.GetMethod("DoAction1");
            MethodDefinition method = hooksClass.GetMethod("PlayerGatherWood");

            this.CloneMethod(orig);
            ILProcessor iLProcessor = orig.Body.GetILProcessor(); // 184 - if (byName != null)
            iLProcessor.InsertBefore(orig.Body.Instructions[184], Instruction.Create(OpCodes.Call, this.rustAssembly.MainModule.Import(method)));
            iLProcessor.InsertBefore(orig.Body.Instructions[184], Instruction.Create(OpCodes.Ldloca_S, orig.Body.Variables[16]));
            iLProcessor.InsertBefore(orig.Body.Instructions[184], Instruction.Create(OpCodes.Ldloca_S, orig.Body.Variables[14]));
            iLProcessor.InsertBefore(orig.Body.Instructions[184], Instruction.Create(OpCodes.Ldloca_S, orig.Body.Variables[17]));
            iLProcessor.InsertBefore(orig.Body.Instructions[184], Instruction.Create(OpCodes.Ldloc_S, orig.Body.Variables[11]));
            iLProcessor.InsertBefore(orig.Body.Instructions[184], Instruction.Create(OpCodes.Ldloc_S, orig.Body.Variables[5]));
        }

        private void PlayerGatherPatch()
        {
            TypeDefinition type = rustAssembly.MainModule.GetType("ResourceTarget");
            MethodDefinition orig = type.GetMethod("DoGather");
            MethodDefinition method = hooksClass.GetMethod("PlayerGather");

            this.CloneMethod(orig);
            ILProcessor iLProcessor = orig.Body.GetILProcessor(); // 30 - int amount = (int) Mathf.Abs(this.gatherProgress);
            iLProcessor.InsertBefore(orig.Body.Instructions[30], Instruction.Create(OpCodes.Call, this.rustAssembly.MainModule.Import(method)));
            iLProcessor.InsertBefore(orig.Body.Instructions[30], Instruction.Create(OpCodes.Ldloca, orig.Body.Variables[1]));
            iLProcessor.InsertBefore(orig.Body.Instructions[30], Instruction.Create(OpCodes.Ldloc_0));
            iLProcessor.InsertBefore(orig.Body.Instructions[30], Instruction.Create(OpCodes.Ldarg_0));
            iLProcessor.InsertBefore(orig.Body.Instructions[30], Instruction.Create(OpCodes.Ldarg_1));
        }

        private void EntityDeployedPatch_DeployableItemDataBlock()
        {
            TypeDefinition type = rustAssembly.MainModule.GetType("DeployableItemDataBlock");
            MethodDefinition orig = type.GetMethod("DoAction1");
            MethodDefinition method = hooksClass.GetMethod("EntityDeployed");
            this.CloneMethod(orig);
            ILProcessor iLProcessor = orig.Body.GetILProcessor(); // 60 - leave (end of try block)
            iLProcessor.InsertBefore(orig.Body.Instructions[60], Instruction.Create(OpCodes.Call, this.rustAssembly.MainModule.Import(method)));
            iLProcessor.InsertBefore(orig.Body.Instructions[60], Instruction.Create(OpCodes.Ldarg_S, orig.Parameters[2]));
            iLProcessor.InsertBefore(orig.Body.Instructions[60], Instruction.Create(OpCodes.Ldloc_S, orig.Body.Variables[8]));
        }

        private void EntityDeployedPatch_StructureComponentDataBlock()
        {
            TypeDefinition type = rustAssembly.MainModule.GetType("StructureComponentDataBlock");
            MethodDefinition orig = type.GetMethod("DoAction1");
            MethodDefinition method = hooksClass.GetMethod("EntityDeployed");
            this.CloneMethod(orig);
            ILProcessor iLProcessor = orig.Body.GetILProcessor(); // 102 - int count = 1;
            iLProcessor.InsertBefore(orig.Body.Instructions[102], Instruction.Create(OpCodes.Call, rustAssembly.MainModule.Import(method)));
            iLProcessor.InsertBefore(orig.Body.Instructions[102], Instruction.Create(OpCodes.Ldarg_S, orig.Parameters[2]));
            iLProcessor.InsertBefore(orig.Body.Instructions[102], Instruction.Create(OpCodes.Ldloc_S, orig.Body.Variables[8]));
        }

        private void BlueprintUsePatch()
        {
            TypeDefinition type = rustAssembly.MainModule.GetType("BlueprintDataBlock");
            MethodDefinition orig = type.GetMethod("UseItem");
            MethodDefinition method = hooksClass.GetMethod("BlueprintUse");

            this.CloneMethod(orig);
            orig.Body.Instructions.Clear();
            orig.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
            orig.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            orig.Body.Instructions.Add(Instruction.Create(OpCodes.Call, rustAssembly.MainModule.Import(method)));
            orig.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
        }

        private void ChatPatch()
        {
            TypeDefinition type = rustAssembly.MainModule.GetType("chat");
            MethodDefinition orig = type.GetMethod("say");
            MethodDefinition method = hooksClass.GetMethod("ChatReceived");

            this.CloneMethod(orig);
            orig.Body.Instructions.Clear();
            orig.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            orig.Body.Instructions.Add(Instruction.Create(OpCodes.Call, rustAssembly.MainModule.Import(method)));
            orig.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
        }

        private MethodDefinition CloneMethod(MethodDefinition orig) // Method Backuping
        {
            MethodDefinition definition = new MethodDefinition(orig.Name + "Original", orig.Attributes, orig.ReturnType);
            foreach (VariableDefinition definition2 in orig.Body.Variables)
            {
                definition.Body.Variables.Add(definition2);
            }
            foreach (ParameterDefinition definition3 in orig.Parameters)
            {
                definition.Parameters.Add(definition3);
            }
            foreach (Instruction instruction in orig.Body.Instructions)
            {
                definition.Body.Instructions.Add(instruction);
            }
            return definition;
        }

        private void ConsolePatch()
        {
            TypeDefinition type = rustAssembly.MainModule.GetType("ConsoleSystem");
            MethodDefinition orig = type.GetMethod("RunCommand");
            MethodDefinition method = hooksClass.GetMethod("ConsoleReceived");

            this.CloneMethod(orig);
            ILProcessor iLProcessor = orig.Body.GetILProcessor();
            for (int i = 0; i < 8; i++)
            {
                iLProcessor.Remove(orig.Body.Instructions[11]);
            }
            iLProcessor.InsertBefore(orig.Body.Instructions[11], Instruction.Create(OpCodes.Ret));
            iLProcessor.InsertBefore(orig.Body.Instructions[11], Instruction.Create(OpCodes.Call, rustAssembly.MainModule.Import(method)));
            iLProcessor.InsertBefore(orig.Body.Instructions[11], Instruction.Create(OpCodes.Ldarg_0));
        }

        private void DoorSharing()
        {
            TypeDefinition type = rustAssembly.MainModule.GetType("DeployableObject");
            MethodDefinition definition2 = type.GetMethod("BelongsTo");
            MethodDefinition method = hooksClass.GetMethod("CheckOwner");

            definition2.Body.Instructions.Clear();
            definition2.Body.Instructions.Add(Instruction.Create(OpCodes.Nop));
            definition2.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            definition2.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
            definition2.Body.Instructions.Add(Instruction.Create(OpCodes.Call, rustAssembly.MainModule.Import(method)));
            definition2.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
        }

        private void EntityHurtPatch()
        {
            TypeDefinition type = rustAssembly.MainModule.GetType("TakeDamage");
            type.GetField("takenodamage").SetPublic(true);
            type.GetField("_health").SetPublic(true);

            MethodDefinition EntityHurt2 = hooksClass.GetMethod("EntityHurt2");
            MethodDefinition ProcessDamageEvent = type.GetMethod("ProcessDamageEvent");
            ProcessDamageEvent.Body.Instructions.Clear();
            ProcessDamageEvent.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            ProcessDamageEvent.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
            ProcessDamageEvent.Body.Instructions.Add(Instruction.Create(OpCodes.Call, rustAssembly.MainModule.Import(EntityHurt2)));
            ProcessDamageEvent.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));

            //ProcessDamageEvent
            /*TypeDefinition type = rustAssembly.MainModule.GetType("StructureComponent");
            TypeDefinition definition2 = rustAssembly.MainModule.GetType("DeployableObject");
            MethodDefinition definition3 = type.GetMethod("OnHurt");
            MethodDefinition definition4 = definition2.GetMethod("OnHurt");
            MethodDefinition method = hooksClass.GetMethod("EntityHurt");

            MethodReference reference = rustAssembly.MainModule.Import(method);
            definition3.Body.Instructions.Clear();
            definition3.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            definition3.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarga_S, definition3.Parameters[0]));
            definition3.Body.Instructions.Add(Instruction.Create(OpCodes.Call, reference));
            definition3.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
            definition4.Body.Instructions.Clear();
            definition4.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            definition4.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarga_S, definition4.Parameters[0]));
            definition4.Body.Instructions.Add(Instruction.Create(OpCodes.Call, reference));
            definition4.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));*/
        }

        private void ItemsTablesLoadedPatch()
        {
            TypeDefinition type = rustAssembly.MainModule.GetType("DatablockDictionary");
            MethodDefinition orig = type.GetMethod("Initialize");
            MethodDefinition method = hooksClass.GetMethod("ItemsLoaded");
            MethodDefinition definition4 = hooksClass.GetMethod("TablesLoaded");

            this.CloneMethod(orig);
            ILProcessor iLProcessor = orig.Body.GetILProcessor();
            for (int i = 0; i < 13; i++)
            {
                iLProcessor.Remove(orig.Body.Instructions[0x17]);
            }
            orig.Body.Instructions[0x24] = Instruction.Create(OpCodes.Callvirt, rustAssembly.MainModule.Import(method));
            iLProcessor.InsertBefore(orig.Body.Instructions[0x24], Instruction.Create(OpCodes.Ldsfld, type.Fields[2]));
            iLProcessor.InsertBefore(orig.Body.Instructions[0x24], Instruction.Create(OpCodes.Ldsfld, type.Fields[1]));
            iLProcessor.InsertBefore(orig.Body.Instructions[0x3f], Instruction.Create(OpCodes.Stsfld, type.Fields[4]));
            iLProcessor.InsertBefore(orig.Body.Instructions[0x3f], Instruction.Create(OpCodes.Callvirt, rustAssembly.MainModule.Import(definition4)));
            iLProcessor.InsertBefore(orig.Body.Instructions[0x3f], Instruction.Create(OpCodes.Ldsfld, type.Fields[4]));
        }

        private void PlayerHurtPatch()
        {
            TypeDefinition type = rustAssembly.MainModule.GetType("HumanBodyTakeDamage");
            MethodDefinition orig = type.GetMethod("Hurt");
            MethodDefinition method = hooksClass.GetMethod("PlayerHurt");
            // OldPlayer Hurt
            //this.CloneMethod(orig);
            //ILProcessor iLProcessor = orig.Body.GetILProcessor();
            //iLProcessor.InsertBefore(orig.Body.Instructions[0], Instruction.Create(OpCodes.Ldarg_1));
            //iLProcessor.InsertAfter(orig.Body.Instructions[0], Instruction.Create(OpCodes.Call, rustAssembly.MainModule.Import(method)));
        }

        private void PlayerJoinLeavePatch()
        {
            TypeDefinition type = rustAssembly.MainModule.GetType("ServerManagement");
            TypeDefinition definition2 = rustAssembly.MainModule.GetType("ConnectionAcceptor");
            MethodDefinition orig = type.GetMethod("OnUserConnected");
            //MethodDefinition method = hooksClass.GetMethod("PlayerConnect");
            MethodDefinition method = hooksClass.GetMethod("ConnectHandler");
            MethodDefinition definition5 = definition2.GetMethod("uLink_OnPlayerDisconnected");
            MethodDefinition definition6 = hooksClass.GetMethod("PlayerDisconnect");

            this.CloneMethod(orig);
            this.CloneMethod(definition5);
            ILProcessor iLProcessor = orig.Body.GetILProcessor();
            iLProcessor.Body.Instructions.Clear();
            //ConnectHandler
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Call, rustAssembly.MainModule.Import(method)));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
            //iLProcessor.InsertBefore(orig.Body.Instructions[0], Instruction.Create(OpCodes.Call, rustAssembly.MainModule.Import(method)));
            //iLProcessor.InsertBefore(orig.Body.Instructions[0], Instruction.Create(OpCodes.Ldarg_1));
            /*iLProcessor.InsertBefore(orig.Body.Instructions[0], Instruction.Create(OpCodes.Brfalse, orig.Body.Instructions[orig.Body.Instructions.Count - 1]));
            iLProcessor.InsertBefore(orig.Body.Instructions[0], Instruction.Create(OpCodes.Call, rustAssembly.MainModule.Import(method)));
            iLProcessor.InsertBefore(orig.Body.Instructions[0], Instruction.Create(OpCodes.Ldarg_1));*/
            iLProcessor = definition5.Body.GetILProcessor();
            iLProcessor.InsertBefore(definition5.Body.Instructions[0], Instruction.Create(OpCodes.Call, rustAssembly.MainModule.Import(definition6)));
            iLProcessor.InsertBefore(definition5.Body.Instructions[0], Instruction.Create(OpCodes.Ldarg_1));
            //iLProcessor.InsertAfter(definition5.Body.Instructions[0x23], Instruction.Create(OpCodes.Ldloc_1));
            //iLProcessor.InsertAfter(definition5.Body.Instructions[0x24], Instruction.Create(OpCodes.Call, rustAssembly.MainModule.Import(definition6)));
        }

        private void ServerSavePatch()
        {
            TypeDefinition type = rustAssembly.MainModule.GetType("ServerSaveManager");
            rustAssembly.MainModule.GetType("save").IsPublic = true;
            MethodDefinition method = hooksClass.GetMethod("ServerSaved");

            type.GetField("_loadedOnce").SetPublic(true);
            type.GetField("_loading").SetPublic(true);
            type.GetMethod("DateTimeFileString").SetPublic(true);
            type.GetMethod("Get").SetPublic(true);
            type.GetMethod("DoSave").SetPublic(true);
            type.GetMethod("DateTimeFileString").SetPublic(true);

            MethodDefinition AutoSave = type.GetMethod("AutoSave");

            ILProcessor iLProcessor = AutoSave.Body.GetILProcessor();
            iLProcessor.Body.Instructions.Clear();
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Callvirt, this.rustAssembly.MainModule.Import(method)));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
        }

        private void PlayerKilledPatch()
        {
            TypeDefinition type = rustAssembly.MainModule.GetType("HumanController");
            MethodDefinition orig = type.GetMethod("OnKilled");
            MethodDefinition method = hooksClass.GetMethod("PlayerKilled");

            this.CloneMethod(orig);
            ILProcessor iLProcessor = orig.Body.GetILProcessor();
            iLProcessor.InsertAfter(orig.Body.Instructions[0x15], Instruction.Create(OpCodes.Ldarga_S, orig.Parameters[0]));
            iLProcessor.InsertAfter(orig.Body.Instructions[0x16], Instruction.Create(OpCodes.Callvirt, rustAssembly.MainModule.Import(method)));
            iLProcessor.InsertAfter(orig.Body.Instructions[0x17], Instruction.Create(OpCodes.Brfalse, orig.Body.Instructions[0x2f]));
            orig.Body.Instructions[0x11] = Instruction.Create(OpCodes.Brfalse, orig.Body.Instructions[0x16]);
        }

        private void PatchuLink()
        {
            AssemblyDefinition ulink = AssemblyDefinition.ReadAssembly("uLink.dll");
            TypeDefinition Class0 = ulink.MainModule.GetType("Class0");
            MethodDefinition method = hooksClass.GetMethod("uLinkCatch");

            Class0.IsPublic = true;
            Class0.GetField("endPoint_0").SetPublic(true);

            MethodDefinition method_17 = Class0.GetMethod("method_17");
            int Position = method_17.Body.Instructions.Count - 34; // second was 30

            ILProcessor iLProcessor = method_17.Body.GetILProcessor();
            iLProcessor.InsertBefore(method_17.Body.Instructions[Position], Instruction.Create(OpCodes.Callvirt, ulink.MainModule.Import(method)));
            iLProcessor.InsertBefore(method_17.Body.Instructions[Position], Instruction.Create(OpCodes.Ldarg_0));

            MethodDefinition method_41 = Class0.GetMethod("method_41");
            int Position2 = method_41.Body.Instructions.Count - 34; // second was 30

            ILProcessor iLProcessor2 = method_41.Body.GetILProcessor();
            iLProcessor2.InsertBefore(method_41.Body.Instructions[Position2], Instruction.Create(OpCodes.Callvirt, ulink.MainModule.Import(method)));
            iLProcessor2.InsertBefore(method_41.Body.Instructions[Position2], Instruction.Create(OpCodes.Ldarg_0));

            ulink.Write("uLink.dll");
        }

        private void TalkerNotifications()
        {
            TypeDefinition type = rustAssembly.MainModule.GetType("VoiceCom");
            type.IsSealed = false;
            MethodDefinition method2 = hooksClass.GetMethod("ShowTalker");

            MethodDefinition clientspeak = type.GetMethod("clientspeak");
            MethodDefinition method = hooksClass.GetMethod("ConfirmVoice");
            
            type.GetField("playerList").SetPublic(true);

            /*ILProcessor iLProcessor = clientspeak.Body.GetILProcessor();
            iLProcessor.Body.Instructions.Clear();
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_2));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Call, this.rustAssembly.MainModule.Import(method)));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));*/

            int i = 0;
            ILProcessor iLProcessor = clientspeak.Body.GetILProcessor();
            iLProcessor.InsertBefore(clientspeak.Body.Instructions[i], Instruction.Create(OpCodes.Ret));
            iLProcessor.InsertBefore(clientspeak.Body.Instructions[i], Instruction.Create(OpCodes.Brtrue_S, clientspeak.Body.Instructions[1]));
            iLProcessor.InsertBefore(clientspeak.Body.Instructions[i], Instruction.Create(OpCodes.Call, this.rustAssembly.MainModule.Import(method)));
            iLProcessor.InsertBefore(clientspeak.Body.Instructions[i], Instruction.Create(OpCodes.Ldarg_2));


            /*ILProcessor iLProcessor = clientspeak.Body.GetILProcessor();
            iLProcessor.Body.Instructions.Clear();
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_2));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Call, rustAssembly.MainModule.Import(method)));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));*/


            VariableDefinition variable1 = clientspeak.Body.Variables[0];
            VariableDefinition variable3 = clientspeak.Body.Variables[6];

            int i2 = clientspeak.Body.Instructions.Count - 42;

            iLProcessor.InsertBefore(clientspeak.Body.Instructions[i2], Instruction.Create(OpCodes.Call, this.rustAssembly.MainModule.Import(method2)));
            iLProcessor.InsertBefore(clientspeak.Body.Instructions[i2], Instruction.Create(OpCodes.Ldloc_S, variable1));
            iLProcessor.InsertBefore(clientspeak.Body.Instructions[i2], Instruction.Create(OpCodes.Ldloc_S, variable3));

            /*MethodDefinition method2 = hooksClass.GetMethod("ShowTalker");
            FieldDefinition field = definition2.GetField("netPlayer");
            VariableDefinition variable = null;
            variable = clientspeak.Body.Variables[6];

            iLProcessor.InsertAfter(clientspeak.Body.Instructions[0x57], Instruction.Create(OpCodes.Ldloc_S, variable));
            iLProcessor.InsertAfter(clientspeak.Body.Instructions[0x58], Instruction.Create(OpCodes.Ldfld, rustAssembly.MainModule.Import(field)));
            iLProcessor.InsertAfter(clientspeak.Body.Instructions[0x59], Instruction.Create(OpCodes.Ldloc_0));
            iLProcessor.InsertAfter(clientspeak.Body.Instructions[90], Instruction.Create(OpCodes.Call, rustAssembly.MainModule.Import(method2)));*/
        }

        private void ConditionDebug()
        {
            TypeDefinition Controllable = rustAssembly.MainModule.GetType("Controllable");
            Controllable.GetField("localPlayerControllableCount").SetPublic(true);
        }

        private void NetcullPatch()
        {
            TypeDefinition NetCull = rustAssembly.MainModule.GetType("NetCull");
            MethodDefinition CloseConnection = NetCull.GetMethod("CloseConnection");
            TypeDefinition logger = fougeriteAssembly.MainModule.GetType("Fougerite.Logger");
            MethodDefinition logex = logger.GetMethod("LogException");

            WrapMethod(CloseConnection, logex, rustAssembly, false);
        }

        private void ShootPatch()
        {
            TypeDefinition BulletWeaponDataBlock = rustAssembly.MainModule.GetType("BulletWeaponDataBlock");
            MethodDefinition DoAction1 = BulletWeaponDataBlock.GetMethod("DoAction1");

            MethodDefinition method = hooksClass.GetMethod("ShootEvent");
            int i = DoAction1.Body.Instructions.Count - 60;
            ILProcessor iLProcessor = DoAction1.Body.GetILProcessor();
            iLProcessor.InsertBefore(DoAction1.Body.Instructions[i], Instruction.Create(OpCodes.Call, this.rustAssembly.MainModule.Import(method)));
            iLProcessor.InsertBefore(DoAction1.Body.Instructions[i], Instruction.Create(OpCodes.Ldloc_S, DoAction1.Body.Variables[10]));
            iLProcessor.InsertBefore(DoAction1.Body.Instructions[i], Instruction.Create(OpCodes.Ldarg_3));
            iLProcessor.InsertBefore(DoAction1.Body.Instructions[i], Instruction.Create(OpCodes.Ldarg_2));
            iLProcessor.InsertBefore(DoAction1.Body.Instructions[i], Instruction.Create(OpCodes.Ldloc_0));
            iLProcessor.InsertBefore(DoAction1.Body.Instructions[i], Instruction.Create(OpCodes.Ldarg_0));
        }

        private void BowShootPatch()
        {
            TypeDefinition BowWeaponDataBlock = rustAssembly.MainModule.GetType("BowWeaponDataBlock");
            MethodDefinition DoAction1 = BowWeaponDataBlock.GetMethod("DoAction1");

            MethodDefinition method = hooksClass.GetMethod("BowShootEvent");

            int i = DoAction1.Body.Instructions.Count - 1;
            ILProcessor iLProcessor = DoAction1.Body.GetILProcessor();
            iLProcessor.InsertBefore(DoAction1.Body.Instructions[i], Instruction.Create(OpCodes.Call, this.rustAssembly.MainModule.Import(method)));
            iLProcessor.InsertBefore(DoAction1.Body.Instructions[i], Instruction.Create(OpCodes.Ldloc_0));
            iLProcessor.InsertBefore(DoAction1.Body.Instructions[i], Instruction.Create(OpCodes.Ldarg_3));
            iLProcessor.InsertBefore(DoAction1.Body.Instructions[i], Instruction.Create(OpCodes.Ldarg_2));
            iLProcessor.InsertBefore(DoAction1.Body.Instructions[i], Instruction.Create(OpCodes.Ldarg_0));
        }

        private void ShotgunShootPatch()
        {
            TypeDefinition ShotgunDataBlock = rustAssembly.MainModule.GetType("ShotgunDataBlock");
            MethodDefinition DoAction1 = ShotgunDataBlock.GetMethod("DoAction1");
            rustAssembly.MainModule.GetType("BulletWeaponDataBlock").GetMethod("ReadHitInfo").SetPublic(true);
            rustAssembly.MainModule.GetType("BulletWeaponDataBlock").GetMethod("ApplyDamage").SetPublic(true);

            MethodDefinition method = hooksClass.GetMethod("ShotgunShootEvent");

            ILProcessor iLProcessor = DoAction1.Body.GetILProcessor();
            iLProcessor.Body.Instructions.Clear();
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_2));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_3));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Call, rustAssembly.MainModule.Import(method)));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
        }

        private void DecayStopPatch()
        {
            TypeDefinition EnvDecay = rustAssembly.MainModule.GetType("EnvDecay");
            TypeDefinition CallBacks = EnvDecay.GetNestedType("Callbacks");
            CallBacks.IsPublic = true;
            CallBacks.GetMethod("RunDecayThink").SetPublic(true);
            IEnumerable<MethodDefinition> Consts = CallBacks.GetConstructors();
            MethodDefinition md = Consts.ToArray()[0];
            md.SetPublic(true);
            md.Body.Instructions.Clear();
            md.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
        }

        private void GrenadePatch()
        {
            TypeDefinition HandGrenadeDataBlock = rustAssembly.MainModule.GetType("HandGrenadeDataBlock");
            MethodDefinition DoAction1 = HandGrenadeDataBlock.GetMethod("DoAction1");

            MethodDefinition method = hooksClass.GetMethod("GrenadeEvent");

            ILProcessor iLProcessor = DoAction1.Body.GetILProcessor();
            iLProcessor.Body.Instructions.Clear();
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_2));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_3));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Call, rustAssembly.MainModule.Import(method)));
            iLProcessor.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
        }

        public bool FirstPass()
        {
            try
            {
                bool flag = true;

                if (rustAssembly.MainModule.GetType("Fougerite_Patched_FirstPass") != null)
                {
                    Logger.Log("Assembly-CSharp.dll is already patched, please use a clean library.");
                    return false;
                }

                try
                {
                    this.FieldsUpdatePatch();
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    flag = false;
                }

                try
                {
                    TypeReference type = AssemblyDefinition.ReadAssembly("mscorlib.dll").MainModule.GetType("System.String");
                    TypeDefinition item = new TypeDefinition("", "Fougerite_Patched_FirstPass", TypeAttributes.AnsiClass | TypeAttributes.Public);
                    rustAssembly.MainModule.Types.Add(item);
                    TypeReference fieldType = rustAssembly.MainModule.Import(type);
                    FieldDefinition definition3 = new FieldDefinition("Version", FieldAttributes.CompilerControlled | FieldAttributes.FamANDAssem | FieldAttributes.Family, fieldType);
                    definition3.HasConstant = true;
                    definition3.Constant = Program.Version;
                    rustAssembly.MainModule.GetType("Fougerite_Patched_FirstPass").Fields.Add(definition3);
                    rustAssembly.Write("Assembly-CSharp.dll");
                }
                catch (Exception ex)
                {
                    flag = false;
                    Logger.Log(ex);
                }
                return flag;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return false;
            }
        }

        public bool SecondPass()
        {
            Logger.Log("Prepraching LatePost method...");
            this.LatePostInTryCatch();
            Logger.Log("Prepatching UpdateDelegate");
            this.UpdateDelegatePatch();
            Logger.Log("Success! Patching other methods...");
            try
            {
                bool flag = true;

                fougeriteAssembly = AssemblyDefinition.ReadAssembly("Fougerite.dll");
                hooksClass = fougeriteAssembly.MainModule.GetType("Fougerite.Hooks");


                if (rustAssembly.MainModule.GetType("Fougerite_Patched_SecondPass") != null)
                {
                    Logger.Log("Assembly-CSharp.dll is already patched, please use a clean library.");
                    return false;
                }

                try
                {
                    this.WrapWildlifeUpdateInTryCatch();
                    this.uLinkLateUpdateInTryCatch();

                    this.BootstrapAttachPatch();
                    this.NPCHurtKilledPatch_BasicWildLifeAI();
                    this.EntityDecayPatch_StructureMaster();
                    this.EntityDecayPatch_EnvDecay();
                    this.NPCHurtPatch_HostileWildlifeAI();
                    this.ServerShutdownPatch();
                    this.ServerSavePatch();
                    this.BlueprintUsePatch();
                    this.EntityDeployedPatch_DeployableItemDataBlock();
                    this.EntityDeployedPatch_StructureComponentDataBlock();
                    this.PlayerGatherWoodPatch();
                    this.PlayerGatherPatch();
                    this.PlayerSpawningSpawnedPatch();
                    this.ChatPatch();
                    this.ConsolePatch();
                    this.PlayerJoinLeavePatch();
                    this.PlayerKilledPatch();
                    this.PlayerHurtPatch();
                    this.EntityHurtPatch();
                    this.ItemsTablesLoadedPatch();
                    this.DoorSharing();
                    this.TalkerNotifications();
                    this.MetaBolismMethodMod();
                    this.CraftingPatch();
                    this.NavMeshPatch();
                    this.ResourceSpawned();
                    this.InventoryModifications();
                    this.InventoryModifications2();
                    this.AirdropPatch();
                    this.ClientConnectionPatch();
                    this.ConnectionAcceptorPatch();
                    this.HumanControllerPatch();
                    this.SupplyCratePatch();
                    this.LatePostInTryCatch2();
                    this.ResearchPatch();
                    this.ConditionDebug();
                    this.NetcullPatch();
                    this.PatchFacePunch();
                    this.ItemPickupHook();
                    this.FallDamageHook();
                    this.LooterPatch();
                    this.UseablePatch();
                    this.ShootPatch();
                    this.BowShootPatch();
                    this.ShotgunShootPatch();
                    this.GrenadePatch();
                    this.PatchuLink();
                    this.SlotOperationPatch();
                    this.RepairBenchEvent();
                    this.DecayStopPatch();
                }
                catch (Exception ex)
                {
                    Logger.Log("Make sure you have a clean uLink.dll from our package.");
                    Logger.Log(ex);
                    flag = false;
                }

                try
                {
                    TypeReference type = AssemblyDefinition.ReadAssembly("mscorlib.dll").MainModule.GetType("System.String");
                    TypeDefinition item = new TypeDefinition("", "Fougerite_Patched_SecondPass", TypeAttributes.AnsiClass | TypeAttributes.Public);
                    rustAssembly.MainModule.Types.Add(item);
                    TypeReference fieldType = rustAssembly.MainModule.Import(type);
                    FieldDefinition definition3 = new FieldDefinition("Version", FieldAttributes.CompilerControlled | FieldAttributes.FamANDAssem | FieldAttributes.Family, fieldType);
                    definition3.HasConstant = true;
                    definition3.Constant = Program.Version;
                    rustAssembly.MainModule.GetType("Fougerite_Patched_SecondPass").Fields.Add(definition3);
                    rustAssembly.Write("Assembly-CSharp.dll");
                }
                catch (Exception ex)
                {
                    flag = false;
                    Logger.Log(ex);
                }
                return flag;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return false;
            }
        }

        private void WrapMethod(MethodDefinition md, MethodDefinition origMethod, AssemblyDefinition asm, bool logEx = false)
        {
            Instruction instruction2;
            ILProcessor iLProcessor = md.Body.GetILProcessor();
            //Instruction instruction = Instruction.Create(OpCodes.Ldarg_0);
            if (md.ReturnType.Name == "Void")
            {
                instruction2 = md.Body.Instructions[md.Body.Instructions.Count - 1];
            }
            else
            {
                instruction2 = md.Body.Instructions[md.Body.Instructions.Count - 2];
            }
            //iLProcessor.InsertBefore(instruction2, instruction);

            iLProcessor.InsertBefore(instruction2, Instruction.Create(OpCodes.Nop));
            iLProcessor.InsertBefore(instruction2, Instruction.Create(OpCodes.Leave, instruction2));

            TypeReference type = AssemblyDefinition.ReadAssembly("mscorlib.dll").MainModule.GetType("System.Exception");
            md.Body.Variables.Add(new VariableDefinition("ex", asm.MainModule.Import(type))); ;

            Instruction instruction = null;
            if (logEx)
            {
                instruction = Instruction.Create(OpCodes.Stloc_0);
                iLProcessor.InsertBefore(instruction2, instruction);
                iLProcessor.InsertBefore(instruction2, Instruction.Create(OpCodes.Nop));
                iLProcessor.InsertBefore(instruction2, Instruction.Create(OpCodes.Ldloc_0));
                iLProcessor.InsertBefore(instruction2, Instruction.Create(OpCodes.Ldnull));

                for (int i = 0; i < md.Parameters.Count; i++)
                {
                    iLProcessor.InsertBefore(instruction2, Instruction.Create(OpCodes.Ldarga_S, md.Parameters[i]));
                }
                iLProcessor.InsertBefore(instruction2, Instruction.Create(OpCodes.Call, asm.MainModule.Import(origMethod)));
            }
            else
            {
                instruction = Instruction.Create(OpCodes.Nop);
                iLProcessor.InsertBefore(instruction2, instruction);
                iLProcessor.InsertBefore(instruction2, Instruction.Create(OpCodes.Nop));
            }
            iLProcessor.InsertBefore(instruction2, Instruction.Create(OpCodes.Nop));
            iLProcessor.InsertBefore(instruction2, Instruction.Create(OpCodes.Leave, instruction2));

            ExceptionHandler item = new ExceptionHandler(ExceptionHandlerType.Catch);
            item.TryStart = md.Body.Instructions[0];
            item.TryEnd = instruction;
            item.HandlerStart = instruction;
            item.HandlerEnd = instruction2;
            if (md.ReturnType.Name != "Void")
            {
                Instruction instruction3 = Instruction.Create(OpCodes.Ret);
                iLProcessor.InsertBefore(instruction2, instruction3);
            }

            item.CatchType = asm.MainModule.Import(type);
            md.Body.ExceptionHandlers.Add(item);
        }
    }
}