/**
 * Javascript inheritence library.
 * From: http://stackoverflow.com/questions/2510342/improving-simple-javascript-inheritance
 * Inspired by: http://dean.edwards.name/weblog/2006/03/base/
 */

/**
* A function that does nothing: to be used when resetting callback handlers.
* @final
*/
EMPTY_FUNCTION = function () {
    // does nothing.
}

var Class =
{
    /**
    * Defines a new class from the specified instance prototype and class
    * prototype.
    *
    * @param {Object} instancePrototype the object literal used to define the
    * member variables and member functions of the instances of the class
    * being defined.
    * @param {Object} classPrototype the object literal used to define the
    * static member variables and member functions of the class being
    * defined.
    *
    * @return {Function} the newly defined class.
    */
    define: function (instancePrototype, classPrototype) {
        /* This is the constructor function for the class being defined */
        var base = function () {
            if (!this.__prototype_chaining
          && base.prototype.initialize instanceof Function)
                base.prototype.initialize.apply(this, arguments);
        }

        base.prototype = instancePrototype || {};

        if (!base.prototype.initialize)
            base.prototype.initialize = EMPTY_FUNCTION;

        for (var property in classPrototype) {
            if (property == 'initialize')
                continue;

            base[property] = classPrototype[property];
        }

        if (classPrototype && (classPrototype.initialize instanceof Function))
            classPrototype.initialize.apply(base);

        function augment(method, derivedPrototype, basePrototype) {
            if ((method == 'initialize')
          && (basePrototype[method].length == 0)) {
                return function () {
                    basePrototype[method].apply(this);
                    derivedPrototype[method].apply(this, arguments);
                }
            }

            return function () {
                this.base = function () {
                    return basePrototype[method].apply(this, arguments);
                };

                return derivedPrototype[method].apply(this, arguments);
                delete this.base;
            }
        }

        /**
        * Provides the definition of a new class that extends the specified
        * <code>parent</code> class.
        *
        * @param {Function} parent the class to be extended.
        * @param {Object} instancePrototype the object literal used to define
        * the member variables and member functions of the instances of the
        * class being defined.
        * @param {Object} classPrototype the object literal used to define the
        * static member variables and member functions of the class being
        * defined.
        *
        * @return {Function} the newly defined class.
        */
        function extend(parent, instancePrototype, classPrototype) {
            var derived = function () {
                if (!this.__prototype_chaining
            && derived.prototype.initialize instanceof Function)
                    derived.prototype.initialize.apply(this, arguments);
            }

            parent.prototype.__prototype_chaining = true;

            derived.prototype = new parent();

            delete parent.prototype.__prototype_chaining;

            for (var property in instancePrototype) {
                if ((instancePrototype[property] instanceof Function)
            && (parent.prototype[property] instanceof Function)) {
                    derived.prototype[property] = augment(property, instancePrototype, parent.prototype);
                }
                else
                    derived.prototype[property] = instancePrototype[property];
            }

            derived.extend = function (instancePrototype, classPrototype) {
                return extend(derived, instancePrototype, classPrototype);
            }

            for (var property in classPrototype) {
                if (property == 'initialize')
                    continue;

                derived[property] = classPrototype[property];
            }

            if (classPrototype && (classPrototype.initialize instanceof Function))
                classPrototype.initialize.apply(derived);

            return derived;
        }

        base.extend = function (instancePrototype, classPrototype) {
            return extend(base, instancePrototype, classPrototype);
        }
        return base;
    }
}