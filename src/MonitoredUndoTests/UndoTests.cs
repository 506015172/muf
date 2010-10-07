﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using System.ComponentModel;
using MonitoredUndo;

namespace MonitoredUndoTests
{

    [TestClass]
    public class Undo_Test
    {

        #region RootDocument Class

        public class RootDocument : INotifyPropertyChanged, ISupportsUndo
        {
            #region Constructors

            public RootDocument()
            {
                this.Bs = new ObservableCollection<ChildB>();
                this.Bs.CollectionChanged += Bs_CollectionChanged;
            }

            #endregion
            #region Properties

            private ChildA _A;
            public ChildA A
            {
                get { return _A; }
                set
                {
                    if (value == _A)
                        return;

                    value.Root = this;

                    DefaultChangeFactory.OnChanging(this, "A", _A, value);

                    _A = value;
                    OnPropertyChanged("A");
                }
            }

            private ObservableCollection<ChildB> _Bs;
            public ObservableCollection<ChildB> Bs
            {
                get { return _Bs; }
                set
                {
                    if (value == _Bs)
                        return;

                    _Bs = value;
                    OnPropertyChanged("Bs");
                }
            }

            #endregion
            #region INotifyPropertyChanged

            /// <summary>
            /// The PropertyChanged event is used by consuming code
            /// (like WPF's binding infrastructure) to detect when
            /// a value has changed.
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Raise the PropertyChanged event for the 
            /// specified property.
            /// </summary>
            /// <param name="propertyName">
            /// A string representing the name of 
            /// the property that changed.</param>
            /// <remarks>
            /// Only raise the event if the value of the property 
            /// has changed from its previous value</remarks>
            protected void OnPropertyChanged(string propertyName)
            {
                // Validate the property name in debug builds
                VerifyProperty(propertyName);

                if (null != PropertyChanged)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            /// <summary>
            /// Verifies whether the current class provides a property with a given
            /// name. This method is only invoked in debug builds, and results in
            /// a runtime exception if the <see cref="OnPropertyChanged"/> method
            /// is being invoked with an invalid property name. This may happen if
            /// a property's name was changed but not the parameter of the property's
            /// invocation of <see cref="OnPropertyChanged"/>.
            /// </summary>
            /// <param name="propertyName">The name of the changed property.</param>
            [System.Diagnostics.Conditional("DEBUG")]
            private void VerifyProperty(string propertyName)
            {
                Type type = this.GetType();

                // Look for a *public* property with the specified name
                System.Reflection.PropertyInfo pi = type.GetProperty(propertyName);
                if (pi == null)
                {
                    // There is no matching property - notify the developer
                    string msg = "OnPropertyChanged was invoked with invalid " +
                                    "property name {0}. {0} is not a public " +
                                    "property of {1}.";
                    msg = String.Format(msg, propertyName, type.FullName);
                    System.Diagnostics.Debug.Fail(msg);
                }
            }

            #endregion
            #region Collection Changed Handlers

            void Bs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                    foreach (ChildB item in e.NewItems)
                        item.Root = this;
                else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                    foreach (ChildB item in e.OldItems)
                        item.Root = null;

                DefaultChangeFactory.OnCollectionChanged(this, "Bs", this.Bs, e);
            }

            #endregion
            #region ISupportsUndo Members

            public object GetUndoRoot()
            {
                return this;
            }

            #endregion
        }

        #endregion
        #region ChildA Class

        public class ChildA : INotifyPropertyChanged, ISupportsUndo
        {
            #region Constructors

            public ChildA()
            {
                _ID = Guid.NewGuid();
            }

            public ChildA(Guid id)
            {
                _ID = id;
            }

            #endregion
            #region Properties

            private Guid _ID;
            public Guid ID
            {
                get { return _ID; }
            }

            private string _Name;
            public string Name
            {
                get { return _Name; }
                set
                {
                    if (value == _Name)
                        return;

                    DefaultChangeFactory.OnChanging(this, "Name", _Name, value);

                    _Name = value;
                    OnPropertyChanged("Name");
                }
            }


            private RootDocument _Root;
            public RootDocument Root
            {
                get { return _Root; }
                set
                {
                    if (value == _Root)
                        return;

                    _Root = value;
                    OnPropertyChanged("Root");
                }
            }

            #endregion
            #region INotifyPropertyChanged

            /// <summary>
            /// The PropertyChanged event is used by consuming code
            /// (like WPF's binding infrastructure) to detect when
            /// a value has changed.
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Raise the PropertyChanged event for the 
            /// specified property.
            /// </summary>
            /// <param name="propertyName">
            /// A string representing the name of 
            /// the property that changed.</param>
            /// <remarks>
            /// Only raise the event if the value of the property 
            /// has changed from its previous value</remarks>
            protected void OnPropertyChanged(string propertyName)
            {
                // Validate the property name in debug builds
                VerifyProperty(propertyName);

                if (null != PropertyChanged)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            /// <summary>
            /// Verifies whether the current class provides a property with a given
            /// name. This method is only invoked in debug builds, and results in
            /// a runtime exception if the <see cref="OnPropertyChanged"/> method
            /// is being invoked with an invalid property name. This may happen if
            /// a property's name was changed but not the parameter of the property's
            /// invocation of <see cref="OnPropertyChanged"/>.
            /// </summary>
            /// <param name="propertyName">The name of the changed property.</param>
            [System.Diagnostics.Conditional("DEBUG")]
            private void VerifyProperty(string propertyName)
            {
                Type type = this.GetType();

                // Look for a *public* property with the specified name
                System.Reflection.PropertyInfo pi = type.GetProperty(propertyName);
                if (pi == null)
                {
                    // There is no matching property - notify the developer
                    string msg = "OnPropertyChanged was invoked with invalid " +
                                    "property name {0}. {0} is not a public " +
                                    "property of {1}.";
                    msg = String.Format(msg, propertyName, type.FullName);
                    System.Diagnostics.Debug.Fail(msg);
                }
            }

            #endregion
            #region ISupportsUndo Members

            public object GetUndoRoot()
            {
                return this.Root;
            }

            #endregion
        }

        #endregion
        #region ChildB Class

        public class ChildB : INotifyPropertyChanged, ISupportsUndo
        {
            #region Constructors

            public ChildB()
            {
                _ID = Guid.NewGuid();
            }

            public ChildB(Guid id)
            {
                _ID = id;
            }

            #endregion
            #region Properties

            private Guid _ID;
            public Guid ID
            {
                get { return _ID; }
            }

            private string _Name;
            public string Name
            {
                get { return _Name; }
                set
                {
                    if (value == _Name)
                        return;

                    DefaultChangeFactory.OnChanging(this, "Name", _Name, value);

                    _Name = value;
                    OnPropertyChanged("Name");
                }
            }


            private RootDocument _Root;
            public RootDocument Root
            {
                get { return _Root; }
                set
                {
                    if (value == _Root)
                        return;

                    _Root = value;
                    OnPropertyChanged("Root");
                }
            }

            #endregion
            #region INotifyPropertyChanged

            /// <summary>
            /// The PropertyChanged event is used by consuming code
            /// (like WPF's binding infrastructure) to detect when
            /// a value has changed.
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Raise the PropertyChanged event for the 
            /// specified property.
            /// </summary>
            /// <param name="propertyName">
            /// A string representing the name of 
            /// the property that changed.</param>
            /// <remarks>
            /// Only raise the event if the value of the property 
            /// has changed from its previous value</remarks>
            protected void OnPropertyChanged(string propertyName)
            {
                // Validate the property name in debug builds
                VerifyProperty(propertyName);

                if (null != PropertyChanged)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            /// <summary>
            /// Verifies whether the current class provides a property with a given
            /// name. This method is only invoked in debug builds, and results in
            /// a runtime exception if the <see cref="OnPropertyChanged"/> method
            /// is being invoked with an invalid property name. This may happen if
            /// a property's name was changed but not the parameter of the property's
            /// invocation of <see cref="OnPropertyChanged"/>.
            /// </summary>
            /// <param name="propertyName">The name of the changed property.</param>
            [System.Diagnostics.Conditional("DEBUG")]
            private void VerifyProperty(string propertyName)
            {
                Type type = this.GetType();

                // Look for a *public* property with the specified name
                System.Reflection.PropertyInfo pi = type.GetProperty(propertyName);
                if (pi == null)
                {
                    // There is no matching property - notify the developer
                    string msg = "OnPropertyChanged was invoked with invalid " +
                                    "property name {0}. {0} is not a public " +
                                    "property of {1}.";
                    msg = String.Format(msg, propertyName, type.FullName);
                    System.Diagnostics.Debug.Fail(msg);
                }
            }

            #endregion
            #region ISupportsUndo Members

            public object GetUndoRoot()
            {
                return this.Root;
            }

            #endregion
        }

        #endregion

        #region Test Setup

        public RootDocument Document1 { get; set; }
        public RootDocument Document2 { get; set; }

        [TestInitialize]
        public void TestSetup()
        {
            Document1 = new RootDocument();
            Document1.A = new ChildA() { Name = "Document1.ChildA" };
            Document1.Bs.Add(new ChildB() { Name = "Document1.ChildB[0]" });
            Document1.Bs.Add(new ChildB() { Name = "Document1.ChildB[1]" });
            Document1.Bs.Add(new ChildB() { Name = "Document1.ChildB[2]" });

            Assert.IsNotNull(Document1.A.Root);
            Assert.AreSame(Document1, Document1.A.Root);
            Assert.AreSame(Document1, Document1.Bs[0].Root);
            Assert.AreSame(Document1, Document1.Bs[1].Root);
            Assert.AreSame(Document1, Document1.Bs[2].Root);

            Document2 = new RootDocument();
            Document2.A = new ChildA() { Name = "Document2.ChildA" };
            Document2.Bs.Add(new ChildB() { Name = "Document2.ChildB[0]" });
            Document2.Bs.Add(new ChildB() { Name = "Document3.ChildB[1]" });
            Document2.Bs.Add(new ChildB() { Name = "Document4.ChildB[2]" });

            UndoService.Current.Clear();
        }

        #endregion

        #region Tests

        [TestMethod]
        public void UndoService_Creates_UndoRoot_For_Document()
        {
            var undoRoot = UndoService.Current[Document1];
            Assert.IsNotNull(undoRoot);
        }

        [TestMethod]
        public void UndoService_Supports_Multiple_Root_Documents()
        {
            var undoRoot1 = UndoService.Current[Document1];
            var undoRoot2 = UndoService.Current[Document2];

            Assert.IsNotNull(undoRoot1);
            Assert.IsNotNull(undoRoot2);
            Assert.AreNotSame(undoRoot1, undoRoot2);
        }

        [TestMethod]
        public void UndoService_Has_Current_Property_for_Singleton_Instance()
        {
            UndoService svc = UndoService.Current;
            UndoService svc2 = UndoService.Current;

            Assert.IsNotNull(svc);
            Assert.IsNotNull(svc2);
            Assert.AreSame(svc, svc2);
        }

        [TestMethod]
        public void UndoService_Does_Not_Support_Null_Document()
        {
            var root = UndoService.Current[null];
            Assert.IsNull(root);
        }

        [TestMethod]
        public void UndoRoot_Supports_Adding_ChangeSets()
        {
            Document1.A.Name = "Updated1";
            Document1.A.Name = "Updated2";

            Assert.AreEqual(2, UndoService.Current[Document1].UndoStack.Count());
        }

        [TestMethod]
        public void UndoRoot_Supports_Adding_ChangeSets_Directly()
        {
            var change = new DelegateChange(Document1.A,
                                    () => Document1.A.Name = "Original",
                                    () => Document1.A.Name = "NewValue",
                                    new Tuple<object, string>(Document1.A, "Name"));

            var undoRoot = UndoService.Current[Document1];

            // var changeSet = new ChangeSet(undoRoot, "Change Document.A.Name", change);

            undoRoot.AddChange(change, "Change Document.A.Name");

            Assert.AreEqual(1, undoRoot.UndoStack.Count());
            Assert.AreEqual(0, undoRoot.RedoStack.Count());

            var origValue = Document1.A.Name;

            undoRoot.Undo();
            Assert.AreEqual("Original", Document1.A.Name);

            undoRoot.Redo();
            Assert.AreEqual("NewValue", Document1.A.Name);
        }

        [TestMethod]
        public void UndoRoot_Prunes_Redo_Stack_When_Adding_New_ChangeSet()
        {
            var orig = Document1.A.Name;

            Document1.A.Name = "Updated1";
            Document1.A.Name = "Updated2";

            var undoRoot = UndoService.Current[Document1];

            Assert.AreEqual(2, undoRoot.UndoStack.Count());
            Assert.AreEqual(0, undoRoot.RedoStack.Count());

            undoRoot.Undo();
            undoRoot.Undo();
            Assert.AreEqual(orig, Document1.A.Name);
            Assert.AreEqual(2, undoRoot.RedoStack.Count());

            Document1.A.Name = "Updated3";

            Assert.AreEqual(1, undoRoot.UndoStack.Count());
            Assert.AreEqual(0, undoRoot.RedoStack.Count());
        }

        [TestMethod]
        public void UndoRoot_Raises_UndoStackChanged_Event_When_ChangeSet_Added()
        {
            var orig = Document1.A.Name;
            var undoRoot = UndoService.Current[Document1];
            var wasCalled = false;

            var callback = new EventHandler((s, e) => wasCalled = true);
            undoRoot.UndoStackChanged += callback;

            try
            {
                Document1.A.Name = "Updated1";
                Assert.IsTrue(wasCalled);

                wasCalled = false;

                undoRoot.Undo();
                Assert.IsTrue(wasCalled);

                wasCalled = false;

                undoRoot.Redo();
                Assert.IsTrue(wasCalled);
            }
            finally
            {
                undoRoot.UndoStackChanged -= callback;
            }
        }

        [TestMethod]
        public void UndoRoot_Raises_RedoStackChanged_Event_When_ChangeSet_Added()
        {
            var orig = Document1.A.Name;
            var undoRoot = UndoService.Current[Document1];
            var wasCalled = false;

            var callback = new EventHandler((s, e) => wasCalled = true);
            undoRoot.RedoStackChanged += callback;

            try
            {
                Document1.A.Name = "Updated1";
                Assert.IsTrue(wasCalled, "Redo stack event should have been called because of the new change pruning the redo stack.");

                wasCalled = false;

                undoRoot.Undo();
                Assert.IsTrue(wasCalled);

                wasCalled = false;

                undoRoot.Redo();
                Assert.IsTrue(wasCalled);
            }
            finally
            {
                undoRoot.UndoStackChanged -= callback;
            }
        }

        [TestMethod]
        public void UndoRoot_Can_Undo_the_Last_ChangeSet()
        {
            var orig = Document1.A.Name;
            var firstChange = "First Change";
            var secondChange = "Second Change";

            Document1.A.Name = firstChange;
            Document1.A.Name = secondChange;

            UndoService.Current[Document1].Undo();

            Assert.AreEqual(firstChange, Document1.A.Name);
        }

        [TestMethod]
        public void UndoRoot_Can_Redo_the_Last_ChangeSet()
        {
            var orig = Document1.A.Name;
            var firstChange = "First Change";
            var secondChange = "Second Change";

            Document1.A.Name = firstChange;
            Document1.A.Name = secondChange;

            UndoService.Current[Document1].Undo();

            Assert.AreEqual(firstChange, Document1.A.Name);

            UndoService.Current[Document1].Redo();

            Assert.AreEqual(secondChange, Document1.A.Name);
        }

        [TestMethod]
        public void UndoRoot_Can_Undo_Multiple_ChangeSets()
        {
            var orig = Document1.A.Name;
            var firstChange = "First Change";
            var secondChange = "Second Change";
            var root = UndoService.Current[Document1];

            Document1.A.Name = firstChange;
            Document1.A.Name = secondChange;

            Assert.AreEqual(2, root.UndoStack.Count());
            Assert.AreEqual(0, root.RedoStack.Count());

            root.Undo(root.UndoStack.Last());

            Assert.AreEqual(orig, Document1.A.Name);
            Assert.AreEqual(0, root.UndoStack.Count());
            Assert.AreEqual(2, root.RedoStack.Count());
        }
        [TestMethod]
        public void UndoRoot_Can_Redo_Multiple_ChangeSets()
        {
            var orig = Document1.A.Name;
            var firstChange = "First Change";
            var secondChange = "Second Change";
            var root = UndoService.Current[Document1];

            Document1.A.Name = firstChange;
            Document1.A.Name = secondChange;

            Assert.AreEqual(2, root.UndoStack.Count());
            Assert.AreEqual(0, root.RedoStack.Count());

            root.Undo();
            root.Undo();

            Assert.AreEqual(orig, Document1.A.Name);
            Assert.AreEqual(0, root.UndoStack.Count());
            Assert.AreEqual(2, root.RedoStack.Count());

            root.Redo(root.RedoStack.Last());

            Assert.AreEqual(secondChange, Document1.A.Name);
            Assert.AreEqual(2, root.UndoStack.Count());
            Assert.AreEqual(0, root.RedoStack.Count());
        }

        [TestMethod]
        public void UndoRoot_Supports_Starting_a_Batch_Of_Changes()
        {
            var orig = Document1.A.Name;
            var firstChange = "First Change";
            var secondChange = "Second Change";
            var root = UndoService.Current[Document1];

            using (new UndoBatch(Document1, "Change Name", false))
            {
                Document1.A.Name = firstChange;
                Document1.A.Name = secondChange;
            }

            Assert.AreEqual(1, root.UndoStack.Count());
            Assert.AreEqual(0, root.RedoStack.Count());

            root.Undo();

            Assert.AreEqual(orig, Document1.A.Name);
            Assert.AreEqual(0, root.UndoStack.Count());
            Assert.AreEqual(1, root.RedoStack.Count());

            root.Redo();

            Assert.AreEqual(secondChange, Document1.A.Name);
            Assert.AreEqual(1, root.UndoStack.Count());
            Assert.AreEqual(0, root.RedoStack.Count());
        }
        [TestMethod]
        public void UndoRoot_Supports_Nested_Batches_Of_Changes()
        {
            var orig = Document1.A.Name;
            var orig2 = Document1.Bs[0].Name;
            var firstChange = "First Change";
            var secondChange = "Second Change";
            var thirdChange = "Third Change";
            var fourthChange = "Fourth Change";
            var root = UndoService.Current[Document1];

            using (new UndoBatch(Document1, "Change Name", false))
            {
                Document1.A.Name = firstChange;
                Document1.A.Name = secondChange;

                using (new UndoBatch(Document1, "Change Collection", false))
                {
                    Document1.Bs[0].Name = thirdChange;
                    Document1.Bs[0].Name = fourthChange;
                }
            }

            Assert.AreEqual(secondChange, Document1.A.Name);
            Assert.AreEqual(fourthChange, Document1.Bs[0].Name);
            Assert.AreEqual(1, root.UndoStack.Count());
            Assert.AreEqual(0, root.RedoStack.Count());

            root.Undo();

            Assert.AreEqual(orig, Document1.A.Name);
            Assert.AreEqual(orig2, Document1.Bs[0].Name);
            Assert.AreEqual(0, root.UndoStack.Count());
            Assert.AreEqual(1, root.RedoStack.Count());

            root.Redo();

            Assert.AreEqual(secondChange, Document1.A.Name);
            Assert.AreEqual(fourthChange, Document1.Bs[0].Name);
            Assert.AreEqual(1, root.UndoStack.Count());
            Assert.AreEqual(0, root.RedoStack.Count());
        }
        //[TestMethod]
        //public void UndoRoot_Ignores_New_ChangeSets_When_Undoing_ChangeSets()
        //{
        //}
        //[TestMethod]
        //public void UndoRoot_Ignores_New_ChangeSets_When_Redoing_ChangeSets()
        //{
        //}

        //[TestMethod]
        //public void UndoRoot_Throws_InvalidOperationException_If_Undo_Attempted_When_In_Batch()
        //{
        //}
        //[TestMethod]
        //public void UndoRoot_Throws_InvalidOperationException_If_Redo_Attempted_When_In_Batch()
        //{
        //}
        //[TestMethod]
        //public void UndoRoot_Throws_InvalidOperationException_If_Undo_Attempted_Where_Specified_ChangeSet_Not_In_Stack()
        //{
        //}
        //[TestMethod]
        //public void UndoRoot_Throws_InvalidOperationException_If_Redo_Attempted_Where_Specified_ChangeSet_Not_In_Stack()
        //{
        //}




        //[TestMethod]
        //public void ChangeSet_Has_Reference_To_UndoRoot()
        //{
        //}
        //[TestMethod]
        //public void ChangeSet_Has_Property_Named_Undone_Indicating_Whether_The_ChangeSet_Has_Been_Undone()
        //{
        //}
        //[TestMethod]
        //public void ChangeSet_Can_Add_Change()
        //{
        //}
        [TestMethod]
        public void ChangeSet_Supports_Consolidating_Changes_For_The_Same_Change_Key()
        {
            var orig = Document1.A.Name;
            var orig2 = Document1.Bs[0].Name;
            var firstChange = "First Change";
            var secondChange = "Second Change";
            var thirdChange = "Third Change";
            var fourthChange = "Fourth Change";
            var root = UndoService.Current[Document1];

            using (new UndoBatch(Document1, "Change Name", true))
            {
                Document1.A.Name = firstChange;
                Document1.A.Name = secondChange;

                using (new UndoBatch(Document1, "Change Collection", true))
                {
                    Document1.Bs[0].Name = thirdChange;
                    Document1.Bs[0].Name = fourthChange;
                }
            }

            Assert.AreEqual(secondChange, Document1.A.Name);
            Assert.AreEqual(fourthChange, Document1.Bs[0].Name);
            Assert.AreEqual(1, root.UndoStack.Count());
            Assert.AreEqual(0, root.RedoStack.Count());

            root.Undo();

            Assert.AreEqual(orig, Document1.A.Name);
            Assert.AreEqual(orig2, Document1.Bs[0].Name);
            Assert.AreEqual(0, root.UndoStack.Count());
            Assert.AreEqual(1, root.RedoStack.Count());

            root.Redo();

            Assert.AreEqual(secondChange, Document1.A.Name);
            Assert.AreEqual(fourthChange, Document1.Bs[0].Name);
            Assert.AreEqual(1, root.UndoStack.Count());
            Assert.AreEqual(0, root.RedoStack.Count());
        }
        //[TestMethod]
        //public void ChangeSet_Can_Undo_Its_Changes()
        //{
        //}
        //[TestMethod]
        //public void ChangeSet_Can_Redo_Its_Changes()
        //{
        //}

        //[TestMethod]
        //public void Change_Has_Reference_To_Target_Instance()
        //{
        //}
        //[TestMethod]
        //public void Change_Has_Action_That_Performs_Undo_Steps()
        //{
        //}
        //[TestMethod]
        //public void Change_Has_Action_That_Performs_Redo_Steps()
        //{
        //}
        //[TestMethod]
        //public void Change_Has_ChangeKey_That_Uniquely_Identifies_The_Undo_Action()
        //{
        //}
        //[TestMethod]
        //public void Change_Can_Merge_With_A_Change_For_The_Same_Key()
        //{
        //}
        //[TestMethod]
        //public void Change_Calls_ISupportUndoNotification_UndoHappened_Method_After_Undo()
        //{
        //}
        //[TestMethod]
        //public void Change_Calls_ISupportUndoNotification_RedoHappened_Method_After_Redo()
        //{
        //}




        //[TestMethod]
        //public void DefaultChangeFactory_Supports_Scalar_Properties()
        //{
        //}
        //[TestMethod]
        //public void DefaultChangeFactory_Supports_Reference_Type_Properties()
        //{
        //}
        //[TestMethod]
        //public void DefaultChangeFactory_Supports_Collection_Adds()
        //{
        //}
        //[TestMethod]
        //public void DefaultChangeFactory_Supports_Collection_MultipleAdds()
        //{
        //}
        //[TestMethod]
        //public void DefaultChangeFactory_Supports_Collection_Removes()
        //{
        //}
        //[TestMethod]
        //public void DefaultChangeFactory_Supports_Collection_MulitpleRemoves()
        //{
        //}
        //[TestMethod]
        //public void DefaultChangeFactory_Supports_Collection_Reset()
        //{
        //}
        //[TestMethod]
        //public void DefaultChangeFactory_Supports_Collection_Move()
        //{
        //}
        //[TestMethod]
        //public void DefaultChangeFactory_Supports_Collection_Replace()
        //{
        //}

        #endregion

    }
}
