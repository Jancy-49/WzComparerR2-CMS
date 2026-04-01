using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WzComparerR2.CharaSim;
using WzComparerR2.Common;
using WzComparerR2.Config;
using WzComparerR2.Controls;
using WzComparerR2.Properties;
using WzComparerR2.WzLib;

namespace WzComparerR2.CharaSimControl
{
    public class CharaSimControlGroup
    {
        public CharaSimControlGroup()
        {
            tooltip = new AfrmTooltip();
            tooltip.TopMost = true;
        }

        private AfrmTooltip quickView;
        public AfrmTooltip tooltip;
        private AfrmItem frmItem;
        private AfrmStat frmStat;
        private AfrmEquip frmEquip;
        private AfrmSkill frmSkill;
        private AfrmUnion frmUnion;
        private AfrmArchive frmArchive;
        private AfrmJob frmJob;
        private Character character;
        private StringLinker stringLinker;
        public AfrmTooltip TooltipQuickView
        {
            get
            {
                if (quickView == null)
                {
                    quickView = new AfrmTooltip();
                    quickView.ObjectMouseMove += new ObjectMouseEventHandler(frmQuickView_ObjectMouseMove);
                    quickView.ObjectMouseLeave += new EventHandler(frmQuickView_ObjectMouseLeave);
                }
                return quickView;
            }
        }

        public AfrmItem UIItem
        {
            get
            {
                if (frmItem == null)
                {
                    frmItem = new AfrmItem();
                    frmItem.KeyDown += new KeyEventHandler(afrm_KeyDown);
                    frmItem.MouseDown += new MouseEventHandler(frmItem_MouseDown);
                    frmItem.DragOver += new DragEventHandler(frmItem_DragOver);
                    frmItem.DragDrop += new DragEventHandler(frmItem_DragDrop);
                    frmItem.ItemMouseMove += new ItemMouseEventHandler(frmItem_ItemMouseMove);
                    frmItem.ItemMouseLeave += new EventHandler(frmItem_ItemMouseLeave);
                    frmItem.Character = this.character;
                }
                return frmItem;
            }
        }

        public AfrmStat UIStat
        {
            get
            {
                if (frmStat == null)
                {
                    frmStat = new AfrmStat();
                    frmStat.KeyDown += new KeyEventHandler(afrm_KeyDown);
                    frmStat.ObjectMouseMove += new ObjectMouseEventHandler(frmStat_ObjectMouseMove);
                    frmStat.ObjectMouseLeave += new EventHandler(frmStat_ObjectMouseLeave);
                    frmStat.Character = this.character;
                }
                return frmStat;
            }
        }

        public AfrmEquip UIEquip
        {
            get
            {
                if (frmEquip == null)
                {
                    frmEquip = new AfrmEquip();
                    frmEquip.KeyDown += new KeyEventHandler(afrm_KeyDown);
                    frmEquip.MouseDown += new MouseEventHandler(frmEquip_MouseDown);
                    frmEquip.DragOver += new DragEventHandler(frmEquip_DragOver);
                    frmEquip.DragDrop += new DragEventHandler(frmEquip_DragDrop);
                    frmEquip.Character = this.character;
                }
                return frmEquip;
            }
        }

        public AfrmSkill UISkill
        {
            get
            {
                if (frmSkill == null)
                {
                    frmSkill = new AfrmSkill();
                    frmSkill.KeyDown += new KeyEventHandler(afrm_KeyDown);
                    frmSkill.ObjectMouseMove += new ObjectMouseEventHandler(frmSkill_ObjectMouseMove);
                    frmSkill.ObjectMouseLeave += new EventHandler(frmSkill_ObjectMouseLeave);
                    frmSkill.Character = this.character;
                }
                return frmSkill;
            }
        }

        public AfrmUnion UIUnion
        {
            get
            {
                if (frmUnion == null)
                {
                    frmUnion = new AfrmUnion();
                    frmUnion.KeyDown += new KeyEventHandler(afrm_KeyDown);
                    frmUnion.Character = this.character;
                }
                return frmUnion;
            }
        }

        public AfrmArchive UIArchive
        {
            get
            {
                if (frmArchive == null)
                {
                    frmArchive = new AfrmArchive();
                    frmArchive.KeyDown += new KeyEventHandler(afrm_KeyDown);
                }
                return frmArchive;
            }
        }

        public AfrmJob UIJob
        {
            get
            {
                if (frmJob == null)
                {
                    frmJob = new AfrmJob();
                    frmJob.KeyDown += new KeyEventHandler(afrm_KeyDown);
                    frmJob.Character = this.character;
                }
                return frmJob;
            }
        }

        public Character Character
        {
            get { return character; }
            set
            {
                this.character = value;
                this.tooltip.Character = value;
                if (frmEquip != null)
                    this.frmEquip.Character = value;
                if (frmStat != null)
                    this.frmStat.Character = value;
                if (frmEquip != null)
                    this.frmEquip.Character = value;
                if (frmSkill != null)
                    this.frmSkill.Character = value;
                if (frmUnion != null)
                    this.frmUnion.Character = value;
                if (frmJob != null)
                    this.frmJob.Character = value;
            }
        }

        public StringLinker StringLinker
        {
            get { return stringLinker; }
            set
            {
                this.stringLinker = value;
                this.tooltip.StringLinker = value;
            }
        }

        public void afrm_KeyDown(object sender, KeyEventArgs e)
        {
            Form frm = sender as Form;
            if (frm == null)
                return;

            switch (e.KeyCode)
            {
                case Keys.Escape:
                    frm.Hide();
                    break;
                case Keys.F1:
                    break;
                case Keys.Up:
                    frm.Top -= 1;
                    break;
                case Keys.Down:
                    frm.Top += 1;
                    break;
                case Keys.Left:
                    frm.Left -= 1;
                    break;
                case Keys.Right:
                    frm.Left += 1;
                    break;
                case Keys.I:
                    frmItem.Visible = !frmItem.Visible;
                    break;
                case Keys.S:
                    frmStat.Visible = !frmStat.Visible;
                    break;
                case Keys.E:
                    frmEquip.Visible = !frmEquip.Visible;
                    break;
                case Keys.K:
                    frmSkill.Visible = !frmSkill.Visible;
                    break;
            }
        }

        private void frmItem_MouseDown(object sender, MouseEventArgs e)
        {
            ItemBase dragItem = frmItem.GetItemByPoint(e.Location);

            if (dragItem != null)
            {
                if (Control.ModifierKeys == Keys.Control)
                {
                    int originIdx = Array.IndexOf<ItemBase>(frmItem.SelectedTab.Items, dragItem);
                    if (originIdx > -1)
                    {
                        frmItem.SelectedTab.Items[originIdx] = null;
                        frmItem.Refresh();
                    }
                }
                else
                {
                    frmItem.DoDragDrop(dragItem, DragDropEffects.Move);
                    tooltip.Visible = false;
                }
            }
        }

        private ItemBase getDragDataItem(IDataObject data)
        {
            ItemBase dragItem;
            if (data != null
                && ((dragItem = data.GetData(typeof(Item)) as Item) != null
                || (dragItem = data.GetData(typeof(Gear)) as Gear) != null))
            {
                return dragItem;
            }
            return null;
        }

        private void frmItem_DragOver(object sender, DragEventArgs e)
        {
            ItemBase dragItem;
            int idx;
            if ((e.AllowedEffect & DragDropEffects.Move) != 0
                && (idx = frmItem.GetItemIndexByPoint(frmItem.PointToClient(new Point(e.X, e.Y)))) != -1
                && (dragItem = getDragDataItem(e.Data)) != null)
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void frmItem_DragDrop(object sender, DragEventArgs e)
        {
            if ((e.Effect & (DragDropEffects.Move)) == 0)
            {
                return;
            }

            ItemBase dragItem;
            int dropIdx, originIdx;
            if ((dragItem = getDragDataItem(e.Data)) == null
                || (originIdx = Array.IndexOf<ItemBase>(frmItem.SelectedTab.Items, dragItem)) == -1)
            {
                return;
            }
            dropIdx = frmItem.GetItemIndexByPoint(frmItem.PointToClient(new Point(e.X, e.Y)));
            if (dropIdx == -1) //移除装备
            {
                frmItem.SelectedTab.Items[originIdx] = null;
            }
            else if (originIdx != dropIdx)
            {
                frmItem.SelectedTab.Items[originIdx] = frmItem.SelectedTab.Items[dropIdx];
                frmItem.SelectedTab.Items[dropIdx] = dragItem;
            }
            else
            {
                return;
            }
            frmItem.Refresh();
        }

        private void frmItem_ItemMouseMove(object sender, ItemMouseEventArgs e)
        {
            if (e.Item == null)
            {
                tooltip.Visible = false;
                return;
            }
            if (e.Item != tooltip.TargetItem)
            {
                tooltip.TargetItem = e.Item;
                tooltip.Refresh();
            }
            Point pos = frmItem.PointToScreen(e.Location);
            pos.Offset(5, 5);
            tooltip.Location = pos;
            tooltip.Visible = true;
            tooltip.BringToFront();
        }

        private void frmItem_ItemMouseLeave(object sender, EventArgs e)
        {
            tooltip.Visible = false;
        }

        private void frmStat_ObjectMouseMove(object sender, ObjectMouseEventArgs e)
        {
            if (e.Obj is Skill && !this.stringLinker.HasValues)
            {
                this.stringLinker.Load(PluginBase.PluginManager.FindWz(Wz_Type.String).GetValueEx<Wz_File>(null),
                    PluginBase.PluginManager.FindWz(Wz_Type.Item).GetValueEx<Wz_File>(null),
                    PluginBase.PluginManager.FindWz(Wz_Type.Etc).GetValueEx<Wz_File>(null),
                    PluginBase.PluginManager.FindWz(Wz_Type.Quest).GetValueEx<Wz_File>(null));
            }
            if (e.Obj == null)
            {
                tooltip.Visible = false;
                return;
            }
            if (e.Obj != tooltip.TargetItem)
            {
                tooltip.TargetItem = e.Obj;
                tooltip.Refresh();
            }
            Point pos = frmStat.PointToScreen(e.Location);
            pos.Offset(5, 5);
            tooltip.Location = pos;
            tooltip.Visible = true;
            tooltip.BringToFront();
        }

        private void frmStat_ObjectMouseLeave(object sender, EventArgs e)
        {
            tooltip.Visible = false;
        }

        public void frmSkill_ObjectMouseMove(object sender, ObjectMouseEventArgs e)
        {
            if (e.Obj is Skill && !this.stringLinker.HasValues)
            {
                this.stringLinker.Load(PluginBase.PluginManager.FindWz(Wz_Type.String).GetValueEx<Wz_File>(null),
                    PluginBase.PluginManager.FindWz(Wz_Type.Item).GetValueEx<Wz_File>(null),
                    PluginBase.PluginManager.FindWz(Wz_Type.Etc).GetValueEx<Wz_File>(null),
                    PluginBase.PluginManager.FindWz(Wz_Type.Quest).GetValueEx<Wz_File>(null));
                tooltip.SkillRender.InputMode = true;
            }
            if (e.Obj == null)
            {
                tooltip.Visible = false;
                return;
            }
            if (e.Obj != tooltip.TargetItem)
            {
                tooltip.TargetItem = e.Obj;
                tooltip.Refresh();
            }
            Point pos = Control.MousePosition;
            pos.Offset(5, 5);
            tooltip.Location = pos;
            tooltip.Visible = true;
            tooltip.BringToFront();
        }

        public void frmSkill_ObjectMouseLeave(object sender, EventArgs e)
        {
            tooltip.Visible = false;
        }

        private void frmQuickView_ObjectMouseMove(object sender, ObjectMouseEventArgs e)
        {
            if (e.Obj == null)
            {
                tooltip.Visible = false;
                return;
            }
            if (e.Obj != tooltip.TargetItem)
            {
                tooltip.TargetItem = e.Obj;
                tooltip.Refresh();
            }
            Point pos = quickView.PointToScreen(e.Location);
            pos.Offset(5, 5);
            tooltip.Location = pos;
            tooltip.Visible = true;
            tooltip.BringToFront();
        }

        private void frmQuickView_ObjectMouseLeave(object sender, EventArgs e)
        {
            tooltip.Visible = false;
        }

        public void frmEquip_MouseDown(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public void frmEquip_DragOver(object sender, DragEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public void frmEquip_DragDrop(object sender, DragEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
